using BooksAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BooksAPI.Startup;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BooksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private string _connStr;
        public AuthorsController(IConfiguration configuration)
        {
            _connStr = configuration.GetConnectionString("DefaultConnection");
        }
        // GET: api/<AuthorsController>
        [HttpGet]
        public IEnumerable<Author> Get()
        {
            var authors = new List<Author>();
            string query = "SELECT * FROM authors where is_hidden = false";
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
                            //var a = sdr["a_surname"];
                            authors.Add(new Author
                            {
                                Id = Convert.ToInt32(sdr["auth_id"]),
                                Name = Convert.ToString(sdr["a_name"]),
                                Surname = Convert.ToString(sdr["a_surname"]),
                                Sex = Convert.ToString(sdr["a_sex"]),
                                Birthdate = Convert.ToDateTime(sdr["a_birthdate"]),
                                Deathdate = (sdr["a_deathdate"] is DBNull) ? null : Convert.ToDateTime(sdr["a_deathdate"]),
                                Bio = Convert.ToString(sdr["a_bio"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return authors;
        }

        // GET api/<AuthorsController>/5
        [HttpGet("{id}")]
        public IEnumerable<Author> Get(int id)
        {
            var author = new List<Author>();
            string query = String.Format("SELECT * FROM authors where is_hidden = false and authors.auth_id = {0}", id);
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
                            //var a = sdr["a_surname"];
                            author.Add(new Author
                            {
                                Id = Convert.ToInt32(sdr["auth_id"]),
                                Name = Convert.ToString(sdr["a_name"]),
                                Surname = Convert.ToString(sdr["a_surname"]),
                                Sex = Convert.ToString(sdr["a_sex"]),
                                Birthdate = Convert.ToDateTime(sdr["a_birthdate"]),
                                Deathdate = (sdr["a_deathdate"] is DBNull) ? null : Convert.ToDateTime(sdr["a_deathdate"]),
                                Bio = Convert.ToString(sdr["a_bio"])
                            });
                        }
                    }
                    con.Close();
                }
            }
            return author;
        }

        // POST api/<AuthorsController>
        [HttpPost]
        public void Post([FromBody] IEnumerable<Author> authInfo)
        {
            
            if (authInfo.Count<Author>() == 0)
            {
                return;
            }
            string query = "INSERT INTO authors (a_name, a_surname, a_sex, a_birthdate, a_deathdate, a_bio, is_hidden) VALUES ";
            foreach (var auth in authInfo)
            {
                if (String.IsNullOrEmpty(auth.Name)) continue;

                auth.Surname = !String.IsNullOrEmpty(auth.Surname) ? Convert.ToString(auth.Surname) : "null";
                auth.Sex = !String.IsNullOrEmpty(auth.Sex) ? Convert.ToString(auth.Sex) : "null";
                auth.Deathdate = !(auth.Deathdate == null) ? Convert.ToDateTime(auth.Deathdate) : Convert.ToDateTime(null);
                auth.Bio = !String.IsNullOrEmpty(auth.Bio) ? Convert.ToString(auth.Bio) : "null";
                query += String.Format("(\'{0}\', \'{1}\', \'{2}\', \'{3}\', \'{4}\', \'{5}\', \'{6}\'),",
                    auth.Name,
                    auth.Surname,
                    auth.Sex,
                    auth.Birthdate.ToString(),
                    auth.Deathdate.ToString(),
                    auth.Bio,
                    auth.IsHidden.ToString());
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

        // DELETE api/<AuthorsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            string query = String.Format("DELETE FROM authors where authors.auth_id = {0}", id);
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
