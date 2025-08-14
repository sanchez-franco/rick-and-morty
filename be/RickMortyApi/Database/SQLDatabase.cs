using Microsoft.EntityFrameworkCore;
using RickMortyApi.Models;

namespace RickMortyApi.Database
{
    public class RickAndMortyDbContext : DbContext
    {
        public RickAndMortyDbContext(DbContextOptions<RickAndMortyDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccountDb> Accounts { get; set; }

        public DbSet<FavoriteDb> Favorites { get; set; }
    }
}
