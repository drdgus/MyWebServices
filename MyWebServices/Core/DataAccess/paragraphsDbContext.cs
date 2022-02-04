using Microsoft.EntityFrameworkCore;
using MyWebServices.Core.DataAccess.Entities;

namespace MyWebServices.Core.DataAccess
{
    public class ParagraphsDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CustomUserElement> CustomUserElements { get; set; }
        public DbSet<UserPattern> UsersPatterns { get; set; }
        public DbSet<UserSettings> UsersSettings { get; set; }

        public ParagraphsDbContext(DbContextOptions<ParagraphsDbContext> options): base(options){ }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Course>().ToTable("Course");
        }
    }
