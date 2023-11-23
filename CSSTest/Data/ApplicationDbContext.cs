using CSSTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CSSTest.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        public DbSet<Fund> Funds { get; set; }
        public DbSet<Value> Value { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fund>().Property<int>("Id");
            modelBuilder.Entity<Fund>().HasKey("Id");

            modelBuilder.Entity<Value>().HasKey("ValueId");
            modelBuilder.Entity<Fund>().HasMany(e => e.FundValues).WithOne(e => e.Fund).HasForeignKey(e => e.FundId).IsRequired();
        }
    }
}
