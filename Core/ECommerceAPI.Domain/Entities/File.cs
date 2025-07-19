using ECommerceAPI.Domain.Entities.Comman;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Domain.Entities
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string Storage { get; set; }
        [NotMapped] // NotMapped ile File'dan oluşturulan Entityler için veritabanında bu alanı tutmayacağız.
        public override DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
