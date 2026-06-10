using System.ComponentModel.DataAnnotations;

namespace StockExchange.Shared.DTOs;

public class UpdateProfileRequestDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
}
