using Microsoft.EntityFrameworkCore;
using server.Models;

namespace server.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<tUploadHistory> tUploadHistory { get; set; }
    }
}
