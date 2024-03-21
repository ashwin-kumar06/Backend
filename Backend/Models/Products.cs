using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set;}
        public string? Condition { get; set; }
        public int StartingPrice { get; set; }
        public int AuctionDuration { get; set;}
        public int SellerId { get; set;}

        [ForeignKey("SellerId")]
        public Users? Users { get; set; }
        public string? Status { get;}
    }
}
