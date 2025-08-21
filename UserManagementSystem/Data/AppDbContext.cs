using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models;

namespace UserManagementSystem.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<ResetPasswordModel> ResetPasswordModels { get; set; }
        public DbSet<LoginViewModel> LoginViewModels { get; set; }
    }
}
