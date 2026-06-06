using System.ComponentModel.DataAnnotations;

namespace StockExchange.Shared.DTOs;

public class LoginRequestDto
{
	[Required]
	[MaxLength(255)]
	public string UsernameOrEmail { get; set; } = string.Empty;

	[Required]
	[MinLength(6)]
	[MaxLength(255)]
	public string Password { get; set; } = string.Empty;
}
