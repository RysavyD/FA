using System;
using System.Data.Entity;
using System.Linq;
using _3F.Model.Extensions;
using _3F.Model.Migrations;
using _3F.Model.Model;

namespace _3F.Model
{
    public sealed class Repository : IRepository
    {
        private readonly Entities _entities;

        public Repository()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Entities, Configuration>());
            _entities = new Entities();
        }

        public IQueryable<T> All<T>() where T : class
        {
            return _entities.Set<T>().AsQueryable();
        }

        public T One<T>(int id) where T : class, IPrimaryKey
        {
            return _entities.Set<T>().Find(id);
        }

        public T One<T>(string id) where T : class, IPrimaryKey
        {
            int iid = Convert.ToInt32(id);
            return _entities.Set<T>().Find(iid);
        }

        public T One<T>(Func<T, bool> predicate) where T : class
        {
            return _entities.Set<T>().FirstOrDefault(predicate);
        }

        T IRepository.OneByHtml<T>(string htmlName)
        {
            return _entities.Set<T>().First(s => s.HtmlName == htmlName);
        }

        public void Delete<T>(T entity) where T : class
        {
            _entities.Set<T>().Remove(entity);
        }

        public IQueryable<T> Where<T>(System.Linq.Expressions.Expression<Func<T, bool>> predicate) where T : class
        {
            return _entities.Set<T>().Where(predicate);
        }

        public void Add<T>(T entity) where T : class, IPrimaryKey
        {
            if (entity.Id == 0)
                _entities.Set<T>().Add(entity);

            Save();
        }

        public void Save()
        {
            _entities.SaveChanges();
        }


        public string SetHtmlName<T>(T entity, string name) where T : class, IHtmlName
        {
            string html = name.StringToHtmlLink();
            var html1 = html;

            if (Where<T>(e => e.HtmlName == html1).Any())
            {
                int index = 2;
                var html2 = html;
                while (Where<T>(e => e.HtmlName == html2 + "-" + index).Any())
                {
                    index++;
                }
                html = html + "-" + index;
            }

            entity.HtmlName = html;

            return html;
        }

        public void Dispose()
        {
            _entities?.Dispose();
        }
    }
}
