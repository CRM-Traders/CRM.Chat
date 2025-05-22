using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using CRM.Chat.Application.Common.Services.Auth;
using CRM.Identity.Domain.Common.Options.Auth;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Chat.Infrastructure.Services.Auth;

public class JwtTokenService(JwtOptions jwtOptions, IServiceProvider serviceProvider) : IJwtTokenService
{
    public ClaimsPrincipal? ValidateToken(string token, out SecurityToken? validatedToken)
    {
        validatedToken = null;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            byte[] publicKeyBytes = Convert.FromBase64String(jwtOptions.PublicKey);

            RSA rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new RsaSecurityKey(rsaPublicKey),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return principal;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}