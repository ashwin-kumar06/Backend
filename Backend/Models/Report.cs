using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public Users? Users { get; set; }

        public int BidId { get; set; }


        [ForeignKey("BidId")]
        public Bid? Bid { get; set; }
        public string? Status { get; set; }
    }
}
