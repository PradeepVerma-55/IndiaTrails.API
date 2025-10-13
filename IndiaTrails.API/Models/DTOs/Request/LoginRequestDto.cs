using System.ComponentModel.DataAnnotations;

namespace IndiaTrails.API.Models.DTOs.Request;

public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}
