namespace Authentication_Authorization.BLL.DTOs.UserDTOs
{
    public class UserRequestBodyDTO
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
    }
}
