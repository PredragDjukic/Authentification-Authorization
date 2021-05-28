namespace Authentication_Authorization.DAL.Models.Constants
{
    public static class RefreshTokenStoredProcedures
    {
        public const string GetValid = "GetValidRefreshTokenByUsername";
        public const string GetInvalid = "GetInvalidRefreshTokenByUsername";
        public const string Add = "AddRefreshToken";
        public const string Update = "UpdateRefreshToken";
    }
}
