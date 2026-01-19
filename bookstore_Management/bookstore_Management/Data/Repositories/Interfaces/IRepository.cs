using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IRepository<T, TKey> where T : class
{
    IQueryable<T> Query(Expression<Func<T, bool>> predicate = null);

    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(TKey id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Delete(T entity);

    Task<int> SaveChangesAsync();
}