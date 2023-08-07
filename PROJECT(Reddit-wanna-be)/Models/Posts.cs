namespace PROJECT_Reddit_wanna_be_.Project.Data.Entities
{
    public class Posts
    {
        public int Id { get; set; }
        public Guid PostID { get; set; }
        public int UserID {get;set; }
        public int TopicID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }    
        public string Username { get; set; }
        public List<Comments> Comments { get; set; }
    }
}
