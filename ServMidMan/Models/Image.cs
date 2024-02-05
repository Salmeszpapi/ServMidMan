using ServMidMan.Interface;
using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class Image : IImage
    {
        [Key]
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string FileName { get; set; }
        public string ProductReferenceId { get; set; }
        public byte[] TestImage { get; set; }
    }
}
