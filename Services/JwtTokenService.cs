using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BiometricClockingAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace BiometricClockingAPI.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Employee employee)
    {
        var secretKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException(
                "JWT secret key is not configured."
            );
        }

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var expiryMinutes = int.TryParse(
            _configuration["Jwt:ExpiryMinutes"],
            out var configuredMinutes
        )
            ? configuredMinutes
            : 60;

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                employee.EmployeeId.ToString()
            ),

            new(
                ClaimTypes.NameIdentifier,
                employee.EmployeeId.ToString()
            ),

            new(
                ClaimTypes.Name,
                $"{employee.Name} {employee.Surname}"
            ),

            new(
                ClaimTypes.Email,
                employee.EmailAddress
            ),

            new(
                ClaimTypes.Role,
                employee.Role
            ),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            )
        };

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        );

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}