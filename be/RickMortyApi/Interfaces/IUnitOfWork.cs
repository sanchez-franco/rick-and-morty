using RickMortyApi.Database;
using System.Linq.Expressions;

namespace RickMortyApi.Interfaces
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IRepository<AccountDb> Accounts { get; }
        IRepository<FavoriteDb> Favorites { get; }
    }

    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        Task AddAsync(T entity);
        void Remove(T entity);
        bool Any(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        IList<T> Where(Expression<Func<T, bool>> predicate);
    }
}
