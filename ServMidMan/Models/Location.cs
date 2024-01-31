using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
	public class Location
	{
		[Key]
		public int Id { get; set; }
		public string Number { get; set; }
	}
}
