using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RoboAdvisorApi.Models;

namespace ERoboServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly EkonRoboDBContext _context;

        public UsersController(EkonRoboDBContext context)
        {
            _context = context;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userDto = new Users();
            var user = await _context.Users
                .Include(c => c.UserCategoryHistory)
                .Include(c => c.UserBaskets)
                .Include(c => c.RebalanceHistory)
                
                .FirstOrDefaultAsync(c => c.RecordId == id);
            user.Hash = "T1";
            //var json = new JavaScriptSerializer().Serialize(user);
            
            
            return Ok(user);
        }       

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
