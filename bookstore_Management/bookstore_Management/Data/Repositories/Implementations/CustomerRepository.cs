using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Core.Enums;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class CustomerRepository : Repository<Customer, string>, ICustomerRepository
    {
        public CustomerRepository(BookstoreDbContext context) : base(context) { }

        public IQueryable<Customer> SearchByName(string keyword)
        {
            return Query(c => c.Name.Contains(keyword) && c.DeletedDate == null);
        }

        public Task<IEnumerable<Customer>> GetByMemberLevelAsync(MemberTier tier)
        {
            return FindAsync(c => c.MemberLevel == tier && c.DeletedDate == null);
        }

        public async Task<Customer> SearchByPhoneAsync(string phone)
        {
            return await DbSet
                .Where(c => c.Phone == phone && c.DeletedDate == null)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UsePointAsync(string customerId, decimal points)
        {
            var customer = await GetByIdAsync(customerId);

            if (customer == null || customer.DeletedDate != null)
                return false;

            if (customer.LoyaltyPoints < points)
                return false;

            customer.LoyaltyPoints -= points;

            Update(customer);
            await SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddPointAsync(string customerId, decimal points)
        {
            var customer = await GetByIdAsync(customerId);

            if (customer == null || customer.DeletedDate != null)
                return false;

            if (points < 0)
                return false;

            customer.LoyaltyPoints += points;

            Update(customer);
            await SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Customer>> GetAllForListViewAsync()
        {
            return await FindAsync(c => c.DeletedDate == null);
        }
    }
}
