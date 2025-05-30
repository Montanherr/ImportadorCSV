using System.ComponentModel.DataAnnotations;
namespace WebContracts.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required]
        [MinLength(10)]
        public string Password { get; set; }
    }
}