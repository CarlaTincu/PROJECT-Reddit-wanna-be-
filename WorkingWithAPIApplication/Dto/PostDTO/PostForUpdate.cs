namespace WorkingWithAPIApplication.Dto.PostDTO
{
    public class PostForUpdate
    {
        public string Content { get; set; }
        public Guid PostID { get; set; }
        public int UserID { get; set; }
        public int TopicID { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
