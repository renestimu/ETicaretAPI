using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = ETicaretAPI.Domain.Entities.File;

namespace ETicaretAPI.Persistence.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }
       


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Changetracker güncelleme olan verileri yakalamamıza olanak sağlar.

            var datas=ChangeTracker.Entries<BaseEntity>();
            foreach (var item in datas)
            {
                _=item.State switch
                {
                    EntityState.Added => item.Entity.CreateDate = DateTime.UtcNow,
                    EntityState.Modified => item.Entity.UpdateDate = DateTime.UtcNow,
                    _=>DateTime.UtcNow
                };
            }


            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
