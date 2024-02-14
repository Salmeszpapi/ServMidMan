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
        public ServiceStatus Approved { get; set; }
        public string Description { get; set; }

    }
    public class ServicesOrdered()
    {
        public List<ServiceWithProduct> Services= new List<ServiceWithProduct>();
    }
    public class ServiceWithProduct() 
    {
        public Service service { get; set; } = new Service();
        public ProductWithByteImages product {  get; set; } = new ProductWithByteImages();
    }
    public enum ServiceStatus
    {
        UnSettled,
        Approved,
        Rejected,
    }
}
