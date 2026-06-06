namespace StockExchange.Shared.DTOs;

public class LoginResponseDto
{
	public bool Success { get; set; }

	public string Message { get; set; } = string.Empty;

	public long UserId { get; set; }

	public string Username { get; set; } = string.Empty;

	public string Role { get; set; } = string.Empty;
}
