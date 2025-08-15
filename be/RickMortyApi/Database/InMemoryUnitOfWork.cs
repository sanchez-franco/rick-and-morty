using RickMortyApi.Interfaces;
using System.Linq.Expressions;

namespace RickMortyApi.Database
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        private readonly IRepository<AccountDb> _accountRepository;
        private readonly IRepository<FavoriteDb> _favoriteRepository;

        public InMemoryUnitOfWork(InMemoryAccountRepository accountRepository, InMemoryFavoriteRepository favoriteRepository)
        {
            _accountRepository = accountRepository;
            _favoriteRepository = favoriteRepository;
        }

        public IRepository<AccountDb> Accounts => _accountRepository;
        public IRepository<FavoriteDb> Favorites => _favoriteRepository;

        public int SaveChanges()
        {
            return 1;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult<int>(1);
        }
    }

    public class InMemoryFavoriteRepository : InMemoryRepository<FavoriteDb>
    {
    }

    public class InMemoryAccountRepository : InMemoryRepository<AccountDb>
    {
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly List<T> _collection = new List<T>();

        public void Add(T entity)
        {
            _collection.Add(entity);
        }

        public Task AddAsync(T entity)
        {
            _collection.Add(entity);

            return Task.CompletedTask;
        }

        public void Remove(T entity)
        {
            _collection.Remove(entity);
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            Func<T, bool> compiledPredicate = predicate.Compile();

            return _collection.Any(compiledPredicate);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            Func<T, bool> compiledPredicate = predicate.Compile();

            return _collection?.FirstOrDefault(compiledPredicate);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public IList<T> Where(Expression<Func<T, bool>> predicate)
        {
            Func<T, bool> compiledPredicate = predicate.Compile();

            return _collection.Where(compiledPredicate).ToList();
        }
    }
}
