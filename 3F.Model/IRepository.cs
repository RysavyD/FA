using System;
using System.Linq;

namespace _3F.Model
{
    public interface IRepository : IDisposable
    {
        IQueryable<T> All<T>() where T : class;
        T One<T>(int id) where T : class, IPrimaryKey;
        T One<T>(string id) where T : class, IPrimaryKey;
        T One<T>(Func<T, bool> predicate) where T : class;
        T OneByHtml<T>(string htmlName) where T : class, IHtmlName;
        void Delete<T>(T entity) where T : class;
        void Add<T>(T entity) where T : class, IPrimaryKey;
        IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class;
        void Save();
        string SetHtmlName<T>(T entity, string name) where T: class, IHtmlName;
    }
}
