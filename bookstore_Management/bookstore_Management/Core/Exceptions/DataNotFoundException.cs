using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Exceptions
{
    internal class DataNotFoundException : BookstoreException
    {
        public DataNotFoundException(string message) : base(message) { }
    }
}
