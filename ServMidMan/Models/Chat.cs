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
        public List<ChatWithPersonImage> Messages { get; set; } = new List<ChatWithPersonImage>();
        public List<User> AllUsers { get; set; }
    }

    public class ChatWithPersonImage : Chat
    {
        public string SenderImage { get; set; }
        public string ReceiverImae { get; set; }
    }
}
