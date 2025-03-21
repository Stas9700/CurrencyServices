namespace Services.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(string username);
}