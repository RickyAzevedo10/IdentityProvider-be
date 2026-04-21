using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IdentityProvider.Application.DTOs;
using IdentityProvider.Application.Interfaces;

namespace IdentityProvider.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login-challenge")]
    public async Task<IActionResult> GetLoginChallenge([FromBody] PartialLoginChallengeRequest request)
    {
        var result = await _authService.GetLoginChallengeAsync(request);
        return Ok(result);
    }

    [HttpPost("login-validate")]
    public async Task<IActionResult> ValidateLogin([FromBody] PartialLoginValidateRequest request)
    {
        var result = await _authService.ValidatePartialLoginAsync(request);
        return result.IsValid ? Ok(result) : BadRequest(result);
    }
}