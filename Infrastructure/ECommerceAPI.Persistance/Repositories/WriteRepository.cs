﻿using ECommerceAPI.Application.Repositories;
using ECommerceAPI.Domain.Entities.Comman;
using ECommerceAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceAPI.Persistance.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {   
        readonly private ECommerceAPIDbContext _context;
        public WriteRepository(ECommerceAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)
        {
            EntityEntry<T> entityEntry =  await Table.AddAsync(model);
            return entityEntry.State == EntityState.Added;
        }

        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await Table.AddRangeAsync(datas);
            return true;
        }

        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry = Table.Remove(model);
            return entityEntry.State == EntityState.Deleted;
        }
        public bool RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);
            return true;
        }

        public async Task<bool> RemoveAsync(string id)
        {
            //T model = await Table.FirstOrDefaultAsync(data => data.Id == Guid.Parse(id));
            T model = await Table.FindAsync(Guid.Parse(id));
            return Remove(model);
        }
        
        public bool Update(T model)
        {
            //Track edilmeyen verileri güncelleyebilmek için Update fonks. vardır. Diğer türlü Db de yer alan verilerin değerlerini değiştirdiğimizde Update otomatik algılanır.
            EntityEntry<T> entityEntry = Table.Update(model);
            return entityEntry.State == EntityState.Modified;
        }
        public async Task<int> SaveAsync()
            => await _context.SaveChangesAsync();


    }
}
