using System;

namespace Authentication_Authorization.BLL.Models
{
    public class RefreshTokenModel
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Username { get; set; }
    }
}
