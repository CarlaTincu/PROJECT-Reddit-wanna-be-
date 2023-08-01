namespace WorkingWithAPIApplication.Dto.PostDTO
{
    public class PostForCreation
    {
        public Guid PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }

        // User information
        public int UserId { get; set; }
        public string UserName { get; set; }

        // Topic information
        public int TopicId { get; set; }
        public string TopicName { get; set; }
    }
}
