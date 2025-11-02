using System.ComponentModel.DataAnnotations;

namespace PHR.Api.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }

    //public record LoginDto(string Email, string Password);
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }


    public record AuthResponseDto(string Token, string RefreshToken);
    
    //public record ApprovalDto(DateTime Start, DateTime End);
}
