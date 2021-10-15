using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private string _connStr;
        public BooksController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection");
        }
        // GET: api/<BooksController>
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            var books = new List<Book>();
            string query = "SELECT * FROM books where is_hidden = false";
            using (var con = new NpgsqlConnection(_connStr))
            {
                using (var cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (var sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            books.Add(new Book
                            {
                                Id = Convert.ToInt32(sdr["book_id"]),
                                Name = Convert.ToString(sdr["b_name"]),
                                ReleaseDate = Convert.ToDateTime(sdr["b_releasedate"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return books;
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public IEnumerable<Book> Get(int id)
        {
            var book = new List<Book>();
            string query = String.Format("SELECT * FROM books where is_hidden = false and books.book_id = {0}", id);
            using (var con = new NpgsqlConnection(_connStr))
            {
                using (var cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    using (var sdr = cmd.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            book.Add(new Book
                            {
                                Id = Convert.ToInt32(sdr["book_id"]),
                                Name = Convert.ToString(sdr["b_name"]),
                                ReleaseDate = Convert.ToDateTime(sdr["b_releasedate"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return book;
        }

        // POST api/<BooksController>
        [HttpPost]
        public void Post([FromBody] IEnumerable<Book> bookInfo)
        {
            if (bookInfo.Count<Book>() == 0)
            {
                return;
            }
            string query = "INSERT INTO books (b_name, b_releasedate, b_desc, is_hidden) VALUES ";
            foreach (var book in bookInfo)
            {
                if (String.IsNullOrEmpty(book.Name)) continue;

                book.Description = !String.IsNullOrEmpty(book.Description) ? Convert.ToString(book.Description) : "null";
                book.IsHidden= Convert.ToBoolean(book.IsHidden);
                
                query += String.Format("(\'{0}\', \'{1}\', \'{2}\', \'{3}\'),",
                    book.Name,
                    book.ReleaseDate.ToString(),
                    book.Description,
                    book.IsHidden.ToString());
            }
            query = query.Substring(0, query.Length - 1);
            query += ";";
            using (var con = new NpgsqlConnection(_connStr))
            {
                using (var cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteScalar();
                    con.Close();
                }
            }
            return;
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string query = String.Format("DELETE FROM books where books.book_id = {0}", id);
            using (var con = new NpgsqlConnection(_connStr))
            {
                using (var cmd = new NpgsqlCommand(query))
                {
                    cmd.Connection = con;
                    con.Open();
                    cmd.ExecuteScalar();
                    con.Close();
                }
            }
            return;
        }
    }
}
