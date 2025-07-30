using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using URLShortener.Domain.Entities;
using URLShortener.Infrastructure.Data.Context;

namespace URLShortener.Infrastructure.Services;

public sealed class JwtService(IConfiguration configuration)
{

    public string GenerateAccessToken(List<Claim> claims, int lifeTimeInMinutes)
    {
        var lifeTime = TimeSpan.FromMinutes(lifeTimeInMinutes);
        
        var key = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!);

        var token = new JwtSecurityToken(
            claims: claims,
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            expires: DateTime.UtcNow.Add(lifeTime),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public JwtPayload GetPayload(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Payload;
    }
    

    
}