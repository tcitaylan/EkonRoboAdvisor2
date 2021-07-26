using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoboAdvisorApi.Models;

namespace ERoboServices.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly EkonRoboDBContext _context;
        
        public AuthRepository(EkonRoboDBContext context)
        {
            _context = context;    
        }
        public async Task<Users> Login(string username, string password)
        {
            var user = await _context.Users
                .Include(c => c.UserCategoryHistory)
                .FirstOrDefaultAsync(x => x.Email == username);

            if(user == null) return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            var login = new Logins{
                UserId= user.RecordId,
                Ip = "",
                Port = "",
                RecordDate = DateTime.Now
            };
            _context.Logins.Add(login);
            _context.SaveChanges();

            return user;
        }

        public async Task<Users> Register(Users user, string password)
        {
            byte[] passwordHash;
            byte[] saltHash;
            
            CreatePasswordHash(password, out passwordHash, out saltHash);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = saltHash;
            user.IsBlocked = false;
            user.RecordDate = DateTime.Now;
            user.CrDate = DateTime.Now.ToString("yyyy-MM-dd");
            user.CrId = 0;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x => x.Email == username)) return true;

            return false;
        }

        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] saltHash)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                saltHash = hmac.Key;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] saltHash)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(saltHash))
            {                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i<computedHash.Length; i++){
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }
     }
}