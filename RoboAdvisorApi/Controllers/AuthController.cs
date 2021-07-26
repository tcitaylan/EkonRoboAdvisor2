using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ERoboServices.Data;
using Helpers.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoboAdvisorApi.Models;

namespace ERoboServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UsersDto usersDto)
        {
            usersDto.Email = usersDto.Email.ToLower();
            if (await _repo.UserExists(usersDto.Email))
                return BadRequest("User already exists");

            var userToCreate = new Users
            {
                Email = usersDto.Email,
                NameSurname = usersDto.NameSurname
            };

            var createdUser = await _repo.Register(userToCreate, usersDto.password);

            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UsersDto usersDto)
        {
            var userFromRepo = await _repo.Login(usersDto.Email.ToLower(), usersDto.password);

            if (userFromRepo == null)
                return Unauthorized();

            var userScore = 0;
            foreach(var t in userFromRepo.UserCategoryHistory)
            {
                userScore = Convert.ToInt32(t.UserScore);
            }
            

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.RecordId.ToString()),
                new Claim(ClaimTypes.Name, userScore.ToString()),
                new Claim(ClaimTypes.Email, userFromRepo.Email)              
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });



        }


    }
}