using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Exceptions
{
    public class AutheticationErrorException : Exception
    {
        public AutheticationErrorException() : base("Kimlik doğrulama hatası!") { }
        public AutheticationErrorException(string message) : base(message) { }
        public AutheticationErrorException(string message, Exception inner) : base(message, inner) { }
    }
}
