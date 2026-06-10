using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;

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
		response.Email = user.Email;
		response.Role = user.Role;

		return response;
	}

	public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
	{
		var username = request?.Username.Trim() ?? string.Empty;
		var email = request?.Email.Trim().ToLowerInvariant() ?? string.Empty;
		var password = request?.Password ?? string.Empty;

		if (username.Length < 3 || username.Length > 100)
		{
			return Failure("Username must contain between 3 and 100 characters.");
		}

		if (email.Length > 255 || !new EmailAddressAttribute().IsValid(email))
		{
			return Failure("Email is invalid.");
		}

		if (password.Length < 6 || password.Length > 255)
		{
			return Failure("Password must contain between 6 and 255 characters.");
		}

		if (await _unitOfWork.Users.ExistsByUsernameAsync(username, cancellationToken))
		{
			return Failure("Username is already in use.");
		}

		if (await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken))
		{
			return Failure("Email is already in use.");
		}

		var now = DateTime.UtcNow;
		var user = new User
		{
			Username = username,
			Email = email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
			Role = "User",
			IsActive = true,
			CreatedAt = now,
			UpdatedAt = now
		};

		await _unitOfWork.Users.AddAsync(user, cancellationToken);
		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return new LoginResponseDto
		{
			Success = true,
			Message = "Registration successful.",
			UserId = user.Id,
			Username = user.Username,
			Email = user.Email,
			Role = user.Role
		};
	}

	public async Task<UserProfileDto> GetProfileAsync(long userId, CancellationToken cancellationToken = default)
	{
		var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
			?? throw new InvalidOperationException("User account was not found.");
		return ToProfile(user);
	}

	public async Task<UserProfileDto> UpdateProfileAsync(
		long userId,
		UpdateProfileRequestDto request,
		CancellationToken cancellationToken = default)
	{
		var username = request?.Username.Trim() ?? string.Empty;
		var email = request?.Email.Trim().ToLowerInvariant() ?? string.Empty;

		if (username.Length < 3 || username.Length > 100)
		{
			throw new InvalidOperationException("Username must contain between 3 and 100 characters.");
		}

		if (email.Length > 255 || !new EmailAddressAttribute().IsValid(email))
		{
			throw new InvalidOperationException("Email is invalid.");
		}

		var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
			?? throw new InvalidOperationException("User account was not found.");

		var usernameOwner = await _unitOfWork.Users.GetByUsernameAsync(username, cancellationToken);
		if (usernameOwner is not null && usernameOwner.Id != userId)
		{
			throw new InvalidOperationException("Username is already in use.");
		}

		var emailOwner = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
		if (emailOwner is not null && emailOwner.Id != userId)
		{
			throw new InvalidOperationException("Email is already in use.");
		}

		user.Username = username;
		user.Email = email;
		_unitOfWork.Users.Update(user);
		await _unitOfWork.SaveChangesAsync(cancellationToken);
		return ToProfile(user);
	}

	private static LoginResponseDto Failure(string message) => new()
	{
		Success = false,
		Message = message
	};

	private static UserProfileDto ToProfile(User user) => new()
	{
		UserId = user.Id,
		Username = user.Username,
		Email = user.Email,
		Role = user.Role,
		IsActive = user.IsActive,
		CreatedAt = user.CreatedAt
	};
}
