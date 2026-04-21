using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Domain.Interfaces;
using IdentityProvider.Infrastructure.Data;
using IdentityProvider.Infrastructure.Identity;
using IdentityProvider.Server.Config;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure - register services via interfaces
builder.Services.AddSingleton<IUsernameStore, UsernameStore>();
builder.Services.AddSingleton<IUserStore, UserStore>();
builder.Services.AddScoped<IAuthService, IdentityProvider.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IUserRegistrationService, IdentityProvider.Infrastructure.Services.UserRegistrationService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

// Infrastructure - OpenIddict core (DbContext, EF Core, Quartz)
builder.Services.AddOpenIddictCore();

// Server - OpenIddict server and validation (ASP.NET Core specific)
builder.Services.AddOpenIddict()
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("connect/authorize")
               .SetIntrospectionEndpointUris("connect/introspect")
               .SetTokenEndpointUris("connect/token");

        options.AllowAuthorizationCodeFlow()
               .AllowRefreshTokenFlow();

        options.AddEncryptionKey(new SymmetricSecurityKey(
            Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

        options.AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// Presentation - configuration
builder.Services.ConfigureKestrel();
builder.Services.AddCorsPolicy();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityProvider Server API v1");
    c.RoutePrefix = "swagger";
});
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/login", async (HttpContext context) =>
{
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(Path.Combine(app.Environment.WebRootPath, "login", "index.html"));
});

// Seed database using interface
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
    await seeder.SeedAsync();
}

await app.RunAsync();
