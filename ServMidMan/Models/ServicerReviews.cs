using System.ComponentModel.DataAnnotations;

namespace ServMidMan.Models
{
    public class ServicerReviews
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string text { get; set; }
        public int CommenterId { get; set; }
        public int Rating { get; set; }
    }
    public class ServiceReviewExtended : ServicerReviews {

        public string UserName { get; set; }
    }

}
