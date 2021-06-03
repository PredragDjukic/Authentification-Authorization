namespace Authentication_Authorization.BLL.DTOs.UserDTOs
{
    public class UserConfirmationDTO
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }
}
