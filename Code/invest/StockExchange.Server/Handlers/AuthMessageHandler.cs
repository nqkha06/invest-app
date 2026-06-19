using System.Threading;
using System.Threading.Tasks;
using StockExchange.Shared.DTOs;
using StockExchange.Server.Services;

namespace StockExchange.Server.Handlers;

public class AuthMessageHandler
{
    private readonly AuthService _authService;

    public AuthMessageHandler(AuthService authService)
    {
        _authService = authService;
    }

    public Task<LoginResponseDto> HandleLoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        return _authService.ValidateLoginAsync(request, cancellationToken);
    }

    public Task<LoginResponseDto> HandleRegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        return _authService.RegisterAsync(request, cancellationToken);
    }

    public Task<UserProfileDto> HandleGetProfileAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _authService.GetProfileAsync(userId, cancellationToken);
    }

    public Task<UserProfileDto> HandleUpdateProfileAsync(
        long userId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return _authService.UpdateProfileAsync(userId, request, cancellationToken);
    }

    public Task<IReadOnlyList<UserProfileDto>> HandleAdminGetUsersAsync(
        long adminUserId,
        CancellationToken cancellationToken = default)
    {
        return _authService.AdminGetUsersAsync(adminUserId, cancellationToken);
    }

    public Task<UserProfileDto> HandleAdminCreateUserAsync(
        long adminUserId,
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return _authService.AdminCreateUserAsync(adminUserId, request, cancellationToken);
    }

    public Task<UserProfileDto> HandleAdminUpdateUserAsync(
        long adminUserId,
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return _authService.AdminUpdateUserAsync(adminUserId, request, cancellationToken);
    }

    public Task<bool> HandleAdminDeleteUserAsync(
        long adminUserId,
        UserDeleteRequestDto request,
        CancellationToken cancellationToken = default)
    {
        return _authService.AdminDeleteUserAsync(adminUserId, request, cancellationToken);
    }
}
