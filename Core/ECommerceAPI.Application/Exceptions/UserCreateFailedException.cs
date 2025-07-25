﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.Exceptions
{
    public class UserCreateFailedException : Exception
    {

        public UserCreateFailedException() : base("User creation failed.")
        {
        }
        public UserCreateFailedException(string message) : base(message)
        {
        }
        public UserCreateFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public UserCreateFailedException(IEnumerable<string> errors) 
            : base($"User creation failed with errors: {string.Join(", ", errors)}")
        {
        }
    }
}
