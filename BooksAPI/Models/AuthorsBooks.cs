using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksAPI.Models
{
    public class AuthorsBooks
    {
        public int BookId { get; set; }
        // foreign key to Book
        public string BookName { get; set; }
        public string AuthorsNames { get; set; }
    }
}
