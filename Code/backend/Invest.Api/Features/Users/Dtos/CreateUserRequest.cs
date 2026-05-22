using System.ComponentModel.DataAnnotations;

namespace Invest.Api.Features.Users.Dtos;

public class CreateUserRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = "";
}
