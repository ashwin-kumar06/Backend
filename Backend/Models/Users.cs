using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        public string? Password { get; set; }
    }
}
