using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Data.Repositories
{
    /// <summary>
    /// Generic repository interface cho CRUD operations
    /// Sử dụng overload để có nhiều tên
    /// </summary>
    /// <typeparam name="T">Class</typeparam>
    /// <typeparam name="TKey">Kiểu dữ lệu khóa chính</typeparam>
    internal interface IRepository<T, TKey> where T : class
    {
        // Get
        IEnumerable<T> GetAll(); // lấy hết thông tin từ bảng
        T Get(TKey id); // lấy thông tin dựa trên khóa chính
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate); // tìm thông tin
        bool Exists(Expression<Func<T, bool>> predicate); // kiểm tra tồn tại
        int Count(); // đếm
        int Count(Expression<Func<T, bool>> predicate); // đếm có điều kiện
        
        // Create
        void Add(T item); // thêm thông tin
        void Add(IEnumerable<T> items); // overload thêm nhiều thông tin
        
        // Update
        void Update(T item); // sửa thông tin
        void Update(IEnumerable<T> items); // overload sửa nhiều thông tin
        
        
        // Save
        int SaveChanges(); // lưu thông tin
        
        
    }
}
