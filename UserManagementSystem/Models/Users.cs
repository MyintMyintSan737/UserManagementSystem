using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementSystem.Models
{
    public class Users
    {
        public int Id { get; set; }
      
        [Required]
        public string? UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        public string? ContactNo { get; set; }

        [Required]
        public string? NRCNo { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [NotMapped]
        public string? CaptchaCode { get; set; }
    }
}
