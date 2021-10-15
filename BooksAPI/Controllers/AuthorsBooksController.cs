using BooksAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsBooksController : ControllerBase
    {
        private string _connStr;
        public AuthorsBooksController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public IEnumerable<AuthorsBooks> Get()
        {
            var authors = new List<AuthorsBooks>();
            string query = "SELECT * FROM books_authors_view";
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
                            authors.Add(new AuthorsBooks
                            {
                                BookId = Convert.ToInt32(sdr["book_id"]),
                                BookName = Convert.ToString(sdr["b_name"]),
                                AuthorsNames = Convert.ToString(sdr["authors"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return authors;
        }

        [HttpGet("bookscount/auth/{auth_id}")]
        public IEnumerable GetAuthsBooksCount(int auth_id)
        {
            var authors = new List<Object>();
            string query = String.Format("SELECT * FROM AUTHORS_BOOKS_COUNT_VIEW " +
                "WHERE auth_id = {0}", auth_id);
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
                            authors.Add(new
                            {
                                AuthorId = Convert.ToInt32(sdr["auth_id"]),
                                AuthorName = Convert.ToString(sdr["a_name"]),
                                AuthorSurname = Convert.ToString(sdr["a_surname"]),
                                BooksCount = Convert.ToString(sdr["books_count"]),
                            });
                        }
                    }
                    con.Close();
                }
            }
            return authors;
        }

        [HttpPost]
        public StatusCodeResult Post([FromBody] IEnumerable<BooksAuthor> relationsInfo)
        {
            if (relationsInfo.Count<BooksAuthor>() == 0)
            {
                return StatusCode(500);
            }
            string query = "INSERT INTO books_authors (book_id, auth_id) VALUES ";
            foreach (var relation in relationsInfo)
            {
                query += String.Format("({0}, {1}),",
                    relation.BookId,
                    relation.AuthorId);
            }
            query = query.Substring(0, query.Length - 1);
            query += ";";
            try
            {
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
            } 
            catch (Exception ex)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
    }
}
