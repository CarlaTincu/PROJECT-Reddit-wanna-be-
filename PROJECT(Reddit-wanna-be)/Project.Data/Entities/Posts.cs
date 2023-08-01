namespace PROJECT_Reddit_wanna_be_.Project.Data.Entities
{
    public class Posts
    {
        public int Id { get; set; }
        public Guid PostID { get; set; }
        public string UserID {get;set; }
        public string TopicID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }    
    }
}
