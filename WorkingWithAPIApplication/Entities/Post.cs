namespace WorkingWithAPIApplication.Entities
{
    public class Post
    {
        public int Id { get; set; }
 
        public Guid PostID { get; set; }
        public int UserID { get; set; }
        public int TopicID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }

        public string Username { get; set; }
    }
}
