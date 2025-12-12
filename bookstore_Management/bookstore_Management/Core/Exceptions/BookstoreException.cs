using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Exceptions
{
    internal class BookstoreException : System.Exception
    {
        public BookstoreException() { }
        public BookstoreException(string message) : base(message) { }
        public BookstoreException(string message, System.Exception inner) : base(message, inner) { }
    }
}
