using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebContracts.Models;

namespace WebContracts.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Import> Imports { get; set; }
    }
}
