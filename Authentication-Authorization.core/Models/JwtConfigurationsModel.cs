namespace Authentication_Authorization.BLL.Models
{
    public class JwtConfigurationsModel
    {
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public int Expiration { get; set; }
    }
}
