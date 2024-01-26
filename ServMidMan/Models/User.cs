using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = "User";
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public UserType TypeOfUser { get; set; }
    }
    public enum UserType
    {
        Servicer,
        Client,
        Both
    }
}
