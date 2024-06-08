using Microsoft.EntityFrameworkCore;

namespace Hospital.Models
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Issue> Issues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Nguyen Van A", Address = "Ha Noi", Description = "Nope", Sex = "Male", Email = "saolaithenhi@gmail.com", PhoneNumber = "0123456789", Role="Admin"},
                new User { Id = 2, Name = "Kwame Adu-Darkwa", Address = "Africa", Description = "Nope", Sex = "Male", Email = "saolaithenhi@gmail.com", PhoneNumber = "0123456789"},
                new User { Id = 3, Name = "Nguyen Van C", Address = "Ha Noi", Description = "Nope", Sex = "Male", Email = "saolaithenhi@gmail.com", PhoneNumber = "0123456789"}
            );
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Department A"},
                new Department { Id = 2, Name = "Department B"},
                new Department { Id = 3, Name = "Department C"}
            );
            modelBuilder.Entity<Account>().HasData(
                new Account { Id = 1, Username = "admin@hospital.com", Password = "1", IsActive = true, UserId = 1 },
                new Account { Id = 2, Username = "staff", Password = "1", IsActive = true, UserId = 2 },
                new Account { Id = 3, Username = "employee", Password = "1", IsActive = true, UserId = 3 }
            );
        }
    }
}
