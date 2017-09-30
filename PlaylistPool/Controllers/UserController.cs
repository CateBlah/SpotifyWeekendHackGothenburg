using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlaylistPool.Models;
using PlaylistPool.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlaylistPool.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        readonly IDatabaseRepository _databaseRepository;
        public UserController(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public IActionResult Post([FromBody]User user)
        {
            _databaseRepository.SaveUser(user);
            return Ok();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
