using Microsoft.EntityFrameworkCore;
using ServMidMan.Models;

namespace ServMidMan.Data
{
    public class DataProviderContext : DbContext
    {
        public DataProviderContext(DbContextOptions<DataProviderContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
    }
}
