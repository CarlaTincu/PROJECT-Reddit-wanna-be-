namespace WorkingWithAPIApplication.Dto.UserDTO
{
    public class UserForUpdate
    {
        public int ID { get; set; }
        public Guid? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
