using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_tbclements.Models;

namespace Fall2024_Assignment3_tbclements.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Fall2024_Assignment3_tbclements.Models.Actor> Actor { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_tbclements.Models.Movie> Movie { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_tbclements.Models.Tweet> Tweet { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_tbclements.Models.Review> Review { get; set; } = default!;
        public DbSet<Fall2024_Assignment3_tbclements.Models.MovieActor> MovieActor { get; set; } = default!;
    }
}
