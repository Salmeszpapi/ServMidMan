using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
	public class Location
	{
		[Key]
		public int Id { get; set; }
		public string Cities { get; set; }
		public string PostalCode { get; set; }
		public string Disctrict { get; set; }
	}
}
