using Microsoft.EntityFrameworkCore;
using RickMortyApi.Models;

namespace RickMortyApi.Database
{
    public class InMemoryDatabase
    {
        public DbSet<AccountDb> Accounts { get; set; }

        public DbSet<FavoriteDb> Favorites { get; set; }
    }
}
