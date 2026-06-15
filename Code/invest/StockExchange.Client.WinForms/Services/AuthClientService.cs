using StockExchange.Shared.DTOs;
using StockExchange.Shared.Network;

namespace StockExchange.Client.WinForms.Services;

public class AuthClientService
{
    private readonly ClientConnectionService _connection;

    public AuthClientService(ClientConnectionService connection)
    {
        _connection = connection;
    }

    public Task<LoginResponseDto> LoginAsync(
        string usernameOrEmail,
        string password,
        CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<LoginRequestDto, LoginResponseDto>(
            MessageType.Login,
            new LoginRequestDto { UsernameOrEmail = usernameOrEmail, Password = password },
            cancellationToken);
    }

    public Task<LoginResponseDto> RegisterAsync(
        string username,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<RegisterRequestDto, LoginResponseDto>(
            MessageType.Register,
            new RegisterRequestDto { Username = username, Email = email, Password = password },
            cancellationToken);
    }

    public Task<UserProfileDto> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<object, UserProfileDto>(
            MessageType.GetProfile,
            new { },
            cancellationToken);
    }

    public Task<UserProfileDto> UpdateProfileAsync(
        string username,
        string email,
        CancellationToken cancellationToken = default)
    {
        return _connection.SendAsync<UpdateProfileRequestDto, UserProfileDto>(
            MessageType.UpdateProfile,
            new UpdateProfileRequestDto { Username = username, Email = email },
            cancellationToken);
    }

    public Task<List<UserProfileDto>> AdminGetUsersAsync(CancellationToken cancellationToken = default)
{
    return _connection.SendAsync<object, List<UserProfileDto>>(
        MessageType.AdminGetUsers,
        new { },
        cancellationToken);
}

    public Task<UserProfileDto> AdminCreateUserAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default)
{
    return _connection.SendAsync<RegisterRequestDto, UserProfileDto>(
        MessageType.AdminCreateUser,
        request,
        cancellationToken);
}

    public Task<UserProfileDto> AdminUpdateUserAsync(
        UpdateProfileRequestDto request,
        CancellationToken cancellationToken = default)
{
    return _connection.SendAsync<UpdateProfileRequestDto, UserProfileDto>(
        MessageType.AdminUpdateUser,
        request,
        cancellationToken);
}
}
