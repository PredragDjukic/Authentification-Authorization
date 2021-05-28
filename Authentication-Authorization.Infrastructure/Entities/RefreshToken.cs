using System;

namespace Authentication_Authorization.DAL.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Username { get; set; }
    }
}
