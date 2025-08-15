using Microsoft.EntityFrameworkCore;
using RickMortyApi.Interfaces;
using System.Linq.Expressions;

namespace RickMortyApi.Database
{
    public class SQLUnitOfWork : IUnitOfWork
    {
        private readonly RickAndMortyDbContext _context;
        private readonly ISQLAccountRepository _accountRepository;
        private readonly ISQLFavoriteRepository _favoriteRepository;

        public SQLUnitOfWork(RickAndMortyDbContext context, SQLAccountRepository accountRepository, SQLFavoriteRepository favoriteRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
            _favoriteRepository = favoriteRepository;
        }

        public IRepository<AccountDb> Accounts => _accountRepository;
        public IRepository<FavoriteDb> Favorites => _favoriteRepository;

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }

    public interface ISQLAccountRepository : IRepository<AccountDb>
    {
    }

    public class SQLAccountRepository : SQLRepository<AccountDb>, ISQLAccountRepository
    {
        public SQLAccountRepository(RickAndMortyDbContext context) : base(context)
        {
        }
    }

    public interface ISQLFavoriteRepository : IRepository<FavoriteDb>
    {
    }

    public class SQLFavoriteRepository : SQLRepository<FavoriteDb>, ISQLFavoriteRepository
    {
        public SQLFavoriteRepository(RickAndMortyDbContext context) : base(context)
        {
        }
    }

    public class SQLRepository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet;

        public SQLRepository(RickAndMortyDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }

        public IList<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }
    }
}
