using System;

namespace Authentication_Authorization.BLL.Models
{
    public class UserConfirmationDTO
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
