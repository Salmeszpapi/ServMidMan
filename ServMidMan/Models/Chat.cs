using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverID { get; set; }
        public string Massege { get; set; }
        public DateTime SendTime { get; set; }
    }
    public class ChatWithPerson()
    {
        public User Partner { get; set; }
        public List<Chat> Messages { get; set; }
        public List<User> AllUsers { get; set; }
    }
}
