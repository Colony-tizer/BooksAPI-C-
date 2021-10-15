using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksAPI.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Surname { get; set; }
        public string? Sex { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime? Deathdate { get; set; }
        public string? Bio { get; set; }
        public bool IsHidden { get; set; }
    }
}
