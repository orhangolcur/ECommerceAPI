using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.DTOs.User
{
    public class CreateUserResponse
    {
        // Response'dan gelen bilgileri katmanlar arasında taşıyabilmek için DTO oluşturduk
        public bool Succeeded { get; set; }
        public string Message { get; set; }
    }
}
