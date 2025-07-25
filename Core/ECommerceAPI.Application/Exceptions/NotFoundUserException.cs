﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Exceptions
{
    public class NotFoundUserException : Exception
    {
        public NotFoundUserException(): base("Kullanıcı adı veya şifre hatalı") { }
        public NotFoundUserException(string message) : base(message) { }
        public NotFoundUserException(string message, Exception inner) : base(message, inner) { }
    }
}
