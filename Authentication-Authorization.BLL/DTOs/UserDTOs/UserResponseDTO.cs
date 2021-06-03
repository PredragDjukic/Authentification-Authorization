namespace Authentication_Authorization.BLL.DTOs.UserDTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SecretId { get; set; }
        public int Role { get; set; }
    }
}
