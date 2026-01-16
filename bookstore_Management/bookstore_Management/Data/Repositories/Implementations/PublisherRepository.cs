using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bookstore_Management.Data.Context;
using bookstore_Management.Data.Repositories.Interfaces;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Implementations
{
    internal class PublisherRepository : Repository<Publisher, string>, IPublisherRepository
    {
        public PublisherRepository(BookstoreDbContext context) : base(context)
        {
        }

        public IEnumerable<Publisher> SearchByName(string name)
        {
            return Find(s => s.Name.Contains(name) && s.DeletedDate == null);
        }

        public string GetNameByPublisherId(string publisherId)
        {
            return Find(s => s.Id == publisherId && s.DeletedDate == null)
                .Select(s => s.Name)
                .FirstOrDefault();
        }

        public Publisher GetByPhone(string phone)
        {
            return Find(s => s.Phone == phone && s.DeletedDate == null).FirstOrDefault();
        }

        public Publisher GetByEmail(string email)
        {
            return Find(s => s.Email == email && s.DeletedDate == null).FirstOrDefault();
        }
        
    }
}
