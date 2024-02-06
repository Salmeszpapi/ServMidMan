using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    [Index(nameof(Email))]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; } = "User";
        public string Email { get; set; }
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public UserType TypeOfUser { get; set; }
        public double Rating { get; set; }
        public int Voters { get; set; }
        public DateTime LastLogin { get; set; }
    }
    public enum UserType
    {
        Servicer,
        Client,
        Both
    }
    public class UserWithRegister : User
    {
        public string Password2 { get; set; }
    }
}
