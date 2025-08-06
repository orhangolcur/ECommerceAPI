using ECommerceAPI.Domain.Entities.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        //ICollection<Order> Orders { get; set; }
    }
}
