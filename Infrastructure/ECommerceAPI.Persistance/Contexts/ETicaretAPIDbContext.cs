using ECommerceAPI.Domain.Entities;
using ECommerceAPI.Domain.Entities.Comman;
using ECommerceAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using File = ECommerceAPI.Domain.Entities.File;

namespace ECommerceAPI.Persistance.Contexts
{
    // IdentityDbContext'i ECommerceAPIDbContext'e vererek Identit mekanizmasının tablolarını da üretebiliriz.
    public class ECommerceAPIDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public ECommerceAPIDbContext(DbContextOptions<ECommerceAPIDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //ChangeTracker = Entity üzerinde yapılan değişikliklerin ya da yeni eklenen verinin yakalanmasını sağlayan propertydir. Update operasyonlarında Track edilen verileri yakalayıp elde etmemizi sağlar.
            var datas = ChangeTracker.Entries<BaseEntity>();
            foreach (var data in datas)
            {
                _ = data.State switch
                {
                    EntityState.Added => data.Entity.CreatedDate = DateTime.UtcNow,
                    EntityState.Modified => data.Entity.UpdatedDate = DateTime.UtcNow,
                    _ => DateTime.UtcNow //Bu satırı koymadığımız zaman silme işleminde  otomatik olarak Modified satırına girer.
                };
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
