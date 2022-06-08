using System.Collections.Generic;
using _3F.BusinessEntities;

namespace _3F.Model.Repositories
{
    public interface IEntityRepository<T> where T: AbstractEntity
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }
}
