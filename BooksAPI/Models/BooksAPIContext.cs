using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Models
{
    public class BooksAPIContext : DbContext
    {
        public BooksAPIContext(DbContextOptions<BooksAPIContext> options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorsBooks> AuthorsBooks { get; set; }

    }
}
