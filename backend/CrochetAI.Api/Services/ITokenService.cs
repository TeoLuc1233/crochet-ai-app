using CrochetAI.Api.Models;

namespace CrochetAI.Api.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}
