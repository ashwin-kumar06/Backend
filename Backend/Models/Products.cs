using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

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
        public string? StartingDate { get; set; }
        public string? EndingDate { get; set; }
        public string? Status { get; set; }
        [NotMapped]
        public IFormFile? ImagePath { get; set; }
        public string? Image {  get; set; }

        [NotMapped]
        public IFormFile? VideoPath { get; set; }
        public string? Video { get; set; }
        public int SellerId { get; set;}

        [ForeignKey("SellerId")]
        public Users? Users { get; set; }
    }
}
