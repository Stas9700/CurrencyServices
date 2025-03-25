using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Common.Interfaces;
using Services.Dto;

namespace UserService;

[ApiController]
[Route("api/[controller]")]
public class UserAuthenticationController: ControllerBase
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IAuthService _authService;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public UserAuthenticationController(
        IJwtTokenGenerator jwtTokenGenerator,
        IAuthService authService, 
        ITokenBlacklistService tokenBlacklistService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _authService = authService;
        _tokenBlacklistService = tokenBlacklistService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthenticationRequest model)
    {
        if (await _authService.RegisterUser(model.Username, model.Password))
        {
            var token = _jwtTokenGenerator.GenerateJwtToken(model.Username);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthenticationRequest model)
    {
        if (await _authService.AuthenticateUser(model.Username, model.Password))
        {
            var token = _jwtTokenGenerator.GenerateJwtToken(model.Username);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
}