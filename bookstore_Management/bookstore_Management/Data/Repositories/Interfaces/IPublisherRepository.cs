using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    internal interface IPublisherRepository : IRepository<Publisher,string>
    {
        IQueryable<Publisher> SearchByName(string keyword);
        Task<string> GetNameByPublisherIdAsync(string publisherId);
        Task<Publisher> GetByPhoneAsync(string phone);
        Task<Publisher> GetByEmailAsync(string email);
    }
}