namespace PROJECT_Reddit_wanna_be_.Project.Data.Entities
{
    public class User
    {
        public int ID { get; set; }
        public Guid UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
