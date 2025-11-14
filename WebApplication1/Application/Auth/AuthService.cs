using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Application.Common;
using WebApplication1.Contracts.Auth;
using WebApplication1.Options;

namespace WebApplication1.Application.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtOptions _options;
    private readonly SymmetricSecurityKey _key;

    public AuthService(
        UserManager<IdentityUser> userManager,
        ILogger<AuthService> logger,
        IOptions<JwtOptions> options)
    {
        _userManager = userManager;
        _logger = logger;
        _options = options.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return ApiResponse<RegisterResponse>.Fail("Email is already taken.");
        }

        var user = new IdentityUser
        {
            UserName = string.IsNullOrWhiteSpace(request.UserName)
                ? request.Email
                : request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToArray();
            _logger.LogWarning("User registration failed: {Errors}", string.Join("; ", errors));
            return ApiResponse<RegisterResponse>.Fail(errors);
        }

        _logger.LogInformation("User registered: {Email}", request.Email);

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email ?? string.Empty
        };

        return ApiResponse<RegisterResponse>.Ok(response);
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return ApiResponse<LoginResponse>.Fail("Invalid email or password.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new LoginResponse
        {
            AccessToken = tokenString,
            ExpiresAtUtc = expires,
            TokenType = "Bearer"
        };

        return ApiResponse<LoginResponse>.Ok(response);
    }

    public Task<ApiResponse<CurrentUserResponse>> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken ct = default)
    {
        if (principal?.Identity is null || !principal.Identity.IsAuthenticated)
        {
            return Task.FromResult(ApiResponse<CurrentUserResponse>.Fail("User is not authenticated."));
        }

        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;
        var name = principal.Identity.Name ?? email ?? id;

        var response = new CurrentUserResponse
        {
            Id = id,
            Email = email,
            Name = name
        };

        return Task.FromResult(ApiResponse<CurrentUserResponse>.Ok(response));
    }
}
