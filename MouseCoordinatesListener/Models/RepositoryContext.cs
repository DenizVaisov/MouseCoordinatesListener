using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MouseCoordinatesListener.Models
{
    public class RepositoryContext : IdentityDbContext
    {
        public RepositoryContext(DbContextOptions options): base(options) { }

        public virtual DbSet<User> Users { get; set; }
    }
}