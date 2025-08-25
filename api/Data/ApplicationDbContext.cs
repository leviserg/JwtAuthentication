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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>(entity =>
            {
                /*
                entity.Property(e => e.Content)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime");
                */
            });
        }
    }

}
