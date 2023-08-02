namespace WorkingWithAPIApplication.Dto.CommentDTO
{
    public class CommentForCreation
    {
        public Guid CommmentID { get; set; }
        public int UserID { get; set; }
        public int PostID { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
