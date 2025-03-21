using System.IdentityModel.Tokens.Jwt;
using CurrencyService.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Common.Interfaces;

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
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // Извлекаем токен из заголовка Authorization
        var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Добавляем токен в черный список
        _tokenBlacklistService.AddToken(token, jwtToken.ValidTo);

        return Ok();
    }
}