using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class PersonalDetails
    {
        [Key]
        public int PersonId { get; set; }
        public long MobileNumber { get; set; }
        public string? Aadhar { get; set; }
        public string? Address { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users? Users { get; set; }
    }
}
