using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public DateTime ApproximetlyFinishDate { get; set; }
        public double Price { get; set; }
        public bool Approved { get; set; } = false;

    }
    public class ServicesOrdered()
    {
        public List<Service> SenderServices= new List<Service>();
        public List<Service> ReceivedServices= new List<Service>();
    }
}
