using ServMidMan.Models;
using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Entity
{
    public class UserEntity
    {
            public int Id { get; set; }
            public string Name { get; set; } = "User";
            public string Email { get; set; }
            public string Password { get; set; }
            public bool EmailConfirmed { get; set; } = false;

            public UserType TypeOfUser { get; set; }
        
    }
}
