using Microsoft.EntityFrameworkCore;
using ServMidMan.Entity;
using ServMidMan.Models;

namespace ServMidMan.Data
{
    public class DataProviderContext : DbContext
    {
        public DataProviderContext(DbContextOptions<DataProviderContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Location> Locations { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<UserEntity>()
        //        .Property(e => e.TypeOfUser)
        //        .HasColumnType("nvarchar"); // specifying nvarchar type for enum

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}