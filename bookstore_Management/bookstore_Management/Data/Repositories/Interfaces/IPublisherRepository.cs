using System;
using System.Collections.Generic;
using bookstore_Management.Models;

namespace bookstore_Management.Data.Repositories.Interfaces
{
    public interface IPublisherRepository : IRepository<Publisher,string>
    {
        IEnumerable<Publisher> SearchByName(string name);
        string GetNameByPublisherId(string publisherId);
        Publisher GetByPhone(string phone);
        Publisher GetByEmail(string email);
    }
}