using System.Threading;
using System.Threading.Tasks;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.DTOs;

namespace StockExchange.Server.Services;

public class AuthService
{
	private readonly IUnitOfWork _unitOfWork;

	public AuthService(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<LoginResponseDto> ValidateLoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
	{
		var response = new LoginResponseDto();

		if (request is null || string.IsNullOrWhiteSpace(request.UsernameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
		{
			response.Success = false;
			response.Message = "Username/email and password are required.";
			return response;
		}

		// Try find by username first, then by email
		var user = await _unitOfWork.Users.GetByUsernameAsync(request.UsernameOrEmail, cancellationToken);
		if (user is null)
		{
			user = await _unitOfWork.Users.GetByEmailAsync(request.UsernameOrEmail, cancellationToken);
		}

		if (user is null)
		{
			response.Success = false;
			response.Message = "Invalid credentials.";
			return response;
		}

		if (!user.IsActive)
		{
			response.Success = false;
			response.Message = "Account is inactive.";
			return response;
		}

		var verified = false;
		try
		{
			verified = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
		}
		catch
		{
			verified = false;
		}

		if (!verified)
		{
			response.Success = false;
			response.Message = "Invalid credentials.";
			return response;
		}

		response.Success = true;
		response.Message = "Login successful.";
		response.UserId = user.Id;
		response.Username = user.Username;
		response.Role = user.Role;

		return response;
	}
}