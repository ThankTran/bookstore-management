using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Data.Repositories
{
    /// <summary>
    /// Class implement từ IRepository
    /// Định nghĩa sơ lược các hàm làm gì
    /// sử dụng linq để truy vấn --> không bị rối khi code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        // bảng dữ liệu và DB context
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        // gán các thông tin cơ bản bằng constructor
        public Repository(DbContext context)
        {
            _dbContext = context;
            _dbSet = context.Set<T>();
        }

        // các function để lấy dữ liệu từ bảng 
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T Get(TKey id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList(); 
        }

        // function để kiểm tra dữ lệu có tồn tại
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Any(predicate);
        }

        // các function dùng để đếm
        public int Count()
        {
            return _dbSet.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate);
        }

        // các function dùng để thêm vào bảng
        public void Add(T item)
        {
            _dbSet.Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            _dbSet.AddRange(items);
        }

        // các function để sửa bảng
        public void Update(T item)
        {
            _dbSet.Attach(item);
            _dbContext.Entry<T>(item).State = EntityState.Modified; // thông báo cho EF biết thông tin này đã bị sửa
        }

        public void Update(IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                Update(i);
            }
        }

        // function để lưu các thay đổi
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
