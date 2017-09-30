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
        public async Task<IEnumerable<User>> Get()
        {
            return await _databaseRepository.GetAllUsersAsync();
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            await _databaseRepository.SaveUserAsync(user);
            return Ok();
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]string userId)
        {
            await _databaseRepository.DeleteUserAsync(userId);
            return Ok();
        }
    }
}
