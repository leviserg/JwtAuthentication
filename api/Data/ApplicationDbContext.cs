using JwtAuthentication.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;


namespace JwtAuthentication.Data
{
    public interface IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
    public class ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
    {
        public DbSet<User> Users { get; set; }
    }

}
