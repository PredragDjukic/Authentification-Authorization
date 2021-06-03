namespace Authentication_Authorization.BLL.DTOs.UserDTOs
{
    public class UserForTokenDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Role { get; set; }
    }
}
