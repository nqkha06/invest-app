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
}
