using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Products? Products { get; set; }
        public int BidderId { get; set; }
            
        [ForeignKey("BidderId")]
        public Users? Users { get; set; }
        public int BidAmount { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? BidStatus { get; set; }
    }
}
