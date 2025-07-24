
using Microsoft.AspNetCore.Identity;

namespace ECommerceAPI.Domain.Entities.Identity
{
    // IdentityUser default değeri string olan Id ile tanımlanır. 
    public class AppUser : IdentityUser<string>
    {
        public string NameSurname { get; set; }
    }
}
