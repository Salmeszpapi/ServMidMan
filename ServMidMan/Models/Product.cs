using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int UserId { get; set; }
        public string Location { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
