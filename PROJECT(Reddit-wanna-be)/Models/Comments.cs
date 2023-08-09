namespace PROJECT_Reddit_wanna_be_.Models
{
    public class Comments
    {
        public int ID { get; set; }
        public Guid CommmentID { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }
        public string Username { get; set; }
    }
}
