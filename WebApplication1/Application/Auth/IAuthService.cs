using System.Security.Claims;
using WebApplication1.Application.Common;
using WebApplication1.Contracts.Auth;

namespace WebApplication1.Application.Auth;

public interface IAuthService
{
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ApiResponse<CurrentUserResponse>> GetCurrentUserAsync(ClaimsPrincipal principal, CancellationToken ct = default);
}
