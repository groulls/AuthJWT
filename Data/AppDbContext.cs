
using AuthReg.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthReg.Data
{
    public class AppDbContext: DbContext
    {

                public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string userName = "Ivan";
            string password = "123";

                         // добавляем роли
            Role adminRole = new Role {RoleID = 1, RoleName = adminRoleName };
            Role userRole = new Role {RoleID = 2, RoleName = userRoleName };
             User adminUser = new User { UserID = 15,UserName=userName,Password = password, RoleId = adminRole.RoleID };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser});
            base.OnModelCreating(modelBuilder);
        }
    }
}
