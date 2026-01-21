using CrochetAI.Api.DTOs;
using CrochetAI.Api.Models;
using CrochetAI.Api.Repositories;
using Microsoft.AspNetCore.Identity;

namespace CrochetAI.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditLogRepository _auditLogRepository;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditLogRepository auditLogRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check if user exists
        var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingUserByEmail != null)
        {
            throw new InvalidOperationException("Email already registered");
        }

        var existingUserByUsername = await _userManager.FindByNameAsync(request.Username);
        if (existingUserByUsername != null)
        {
            throw new InvalidOperationException("Username already taken");
        }

        // Create user
        var user = new ApplicationUser
        {
            UserName = request.Username,
            Email = request.Email,
            SubscriptionTier = "Free"
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        // Generate tokens
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        // Log registration
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "Register",
            IpAddress = "", // Will be set by controller
            UserAgent = "",
            Timestamp = DateTime.UtcNow
        });

        return new AuthResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                SubscriptionTier = user.SubscriptionTier,
                CreatedAt = user.CreatedAt
            },
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            // Log failed attempt
            await _auditLogRepository.AddAsync(new AuditLog
            {
                UserId = user.Id,
                Action = "LoginFailed",
                IpAddress = "",
                UserAgent = "",
                Timestamp = DateTime.UtcNow
            });

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Revoke old refresh tokens
        await _refreshTokenRepository.RevokeAllUserTokensAsync(user.Id);

        // Generate new tokens
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        // Log successful login
        await _auditLogRepository.AddAsync(new AuditLog
        {
            UserId = user.Id,
            Action = "Login",
            IpAddress = "",
            UserAgent = "",
            Timestamp = DateTime.UtcNow
        });

        return new AuthResponse
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                SubscriptionTier = user.SubscriptionTier,
                CreatedAt = user.CreatedAt
            },
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var isValid = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
        if (!isValid)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        // Get token hash and find user
        var tokenHash = ComputeSha256Hash(refreshToken);
        var token = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        if (token == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        var user = await _userManager.FindByIdAsync(token.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Revoke old token
        await _tokenService.RevokeRefreshTokenAsync(refreshToken);

        // Generate new tokens
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(string userId)
    {
        await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
