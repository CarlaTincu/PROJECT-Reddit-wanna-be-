namespace WorkingWithAPIApplication.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public Guid PostID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }

        //foreign keys
        public string UserID { get; set; }
        public string TopicID { get; set; }

        //related objects
        public User User { get; set; }
        public Topic Topic { get; set; }
    }
}
