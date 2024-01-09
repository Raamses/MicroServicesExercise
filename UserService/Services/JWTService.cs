using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IJWTService
{
    string GenerateToken(int userId, string username);
    string GetJWTTokenFromHeader();
}

public class JWTService : IJWTService
{
    private readonly string _secretKey;
    private readonly ILogger<JWTService> _logger;
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JWTService(ILogger<JWTService> logger, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _config = config;
        _secretKey = config["SecretKey"] ?? string.Empty;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateToken(int userId, string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            }),
            Expires = DateTime.UtcNow.AddDays(7), // Set token expiration as needed
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
    public string GetJWTTokenFromHeader()
    {
        return _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    }
}