using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Interface;

public class Jasonwt : IToken   // inheritance 
{
    private readonly string _secretKey;    // 

    public Jasonwt()
    {
        try
        {
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
        }
        catch (Exception ex)
        {

            throw new InvalidOperationException("ENV failed to load", ex);
        }
        _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
        ?? throw new ArgumentException("Failed to load env");

        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new InvalidOperationException("SECRET_KEY environment variable is not set.");
        }

    }

    public string CreateToken(string userId, string email, string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier,userId),
                new(ClaimTypes.Email,email),
                new(ClaimTypes.Name,username)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);

    }
}