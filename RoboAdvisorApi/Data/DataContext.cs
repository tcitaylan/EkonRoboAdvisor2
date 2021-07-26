using Microsoft.EntityFrameworkCore;
using RoboAdvisorApi.Models;

namespace ERoboServices.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) { }
        
        public DbSet<Users> Users { get; set; }

    }
}