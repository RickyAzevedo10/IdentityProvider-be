using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using IdentityProvider.Application.Interfaces;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityProvider.Server.Controllers;

/// <summary>
/// Authentication endpoints for login, registration and logout
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and creates a session cookie
    /// </summary>
    /// <param name="username">The username of the user</param>
    /// <param name="password">The password of the user</param>
    /// <param name="returnUrl">Optional URL to redirect after successful login</param>
    /// <returns>Redirect to returnUrl or home page</returns>
    /// <response code="302">Redirect on success</response>
    /// <response code="401">Invalid credentials</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password, [FromForm] string? returnUrl)
    {
        var result = await _authService.LoginAsync(username, password);
        if (!result.Success)
            return Unauthorized(result.Error);

        var claims = new List<Claim>
        {
            new(Claims.Subject, result.SubjectId!),
            new(Claims.Name, result.Username!),
            new(Claims.PreferredUsername, result.Username!)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme,
            Claims.Name,
            Claims.Role);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false });

        if (!string.IsNullOrEmpty(returnUrl))
            return Redirect(returnUrl);

        return Redirect("/");
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration data</param>
    /// <returns>Registration result</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="409">Username already exists</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Application.DTOs.RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request.Username, request.Password);
        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "User registered successfully.", username = request.Username });
    }

    /// <summary>
    /// Logs out the current user by removing the session cookie
    /// </summary>
    /// <returns>Redirect to home page</returns>
    /// <response code="302">Redirect to home</response>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
