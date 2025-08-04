using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Application.ViewModels.Baskets
{
    public class VM_Create_BasketItem
    {
        // Burada BasketId almıyoruz çünkü kullanıcının birden fazla Basket'ı olabilir ve bazıları pasif 1 tanesi de aktif olabilir. Bizim bunu backend'de işlememiz gerekiyor, yani client'dan almamız gerekiyor. Aktif olan Basket'ı Service'deki sınıfta ele alıp işlemlerimizi orada yapmalıyız.
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
