using System;

namespace Authentication_Authorization.BLL.Exceptions
{
    public class BussinesException : Exception
    {
        public BussinesException(string message, int statusCode) : base(message)
        {
            this.StatusCode = statusCode;
        }
        public int StatusCode { get; set; }
    }
}
