using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using IdentityProvider.Application.Interfaces;
using IdentityProvider.Application.Services;
using IdentityProvider.Domain.Interfaces;
using IdentityProvider.Infrastructure.Data;
using IdentityProvider.Infrastructure.Identity;
using IdentityProvider.Infrastructure.Persistence.Context;
using IdentityProvider.Infrastructure.Persistence.Interfaces;
using IdentityProvider.Infrastructure.Persistence.UnitOfWork;
using IdentityProvider.Infrastructure.Services;
using IdentityProvider.Server.Config;

var builder = WebApplication.CreateBuilder(args);

// Database - ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(30);
        });

    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// Infrastructure - Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Infrastructure - Services
builder.Services.AddScoped<IUsernameStore, UsernameStore>();
builder.Services.AddScoped<IUserStore, UserStore>();

// Application - Services
builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();

// Infrastructure - Auth Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

// Infrastructure - OpenIddict core (DbContext, EF Core, Quartz)
builder.Services.AddOpenIddictCore(builder.Configuration);

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
