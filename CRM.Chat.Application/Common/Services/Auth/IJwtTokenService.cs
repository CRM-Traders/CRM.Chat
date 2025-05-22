using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace CRM.Chat.Application.Common.Services.Auth;

public interface IJwtTokenService
{
    ClaimsPrincipal? ValidateToken(string token, out SecurityToken? validatedToken);
}