using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services.Common;

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient"; // потребитель токена
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) => 
        new (Encoding.UTF8.GetBytes(key));
}