using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityProvider.Server.Controllers;

/// <summary>
/// Protected resource endpoints
/// </summary>
[ApiController]
public class ResourceController : Controller
{
    /// <summary>
    /// Returns the authenticated user profile information
    /// </summary>
    /// <returns>User profile data including username, subject and scopes</returns>
    /// <response code="200">Returns the user profile</response>
    /// <response code="401">Missing or invalid access token</response>
    [HttpGet("api/profile")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            message = "You have accessed a protected resource",
            username = User.Identity!.Name,
            subject = User.FindFirst(Claims.Subject)?.Value,
            scopes = User.FindFirst(Claims.Scope)?.Value?.Split(' ') ?? []
        });
    }
}
