using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using _3F.Model.Extensions;
using EventCategory = _3F.BusinessEntities.EventCategory;

namespace _3F.Model.Repositories
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        public IEnumerable<EventCategory> GetAll()
        {
            var sql = "SELECT cat.[Id] ,cat.[Name] ,cat.[HtmlName] ,cat.[MainCategory]"
                      + " ,(COUNT(ec.Id_EventCategory)) AS EventCount"
                      + " FROM [dbo].[EventCategory] cat"
                      + " LEFT JOIN EventCategories ec ON ec.Id_EventCategory = cat.Id"
                      + " GROUP BY cat.Id, cat.Name, cat.HtmlName, cat.MainCategory";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var entities = sqlConnection.Query<EventCategory>(sql);
                sqlConnection.Close();
                return entities;
            }
        }

        public EventCategory GetById(int id)
        {
            var sql = "SELECT cat.[Id] ,cat.[Name] ,cat.[HtmlName] ,cat.[MainCategory]"
                      + " FROM [dbo].[EventCategory] cat"
                      + " WHERE cat.[Id] = @id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var entity = sqlConnection.QueryFirst<EventCategory>(sql, new {id});
                sqlConnection.Close();
                return entity;
            }
        }

        public void Add(EventCategory entity)
        {
            entity.HtmlName = GetHtmlName(entity.Name, "EventCategory");

            var sql = "INSERT INTO [dbo].[EventCategory] ([Name], [HtmlName], [MainCategory])"
                      + " VALUES (@Name, @HtmlName, @mainCategory)";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new {entity.Name, entity.HtmlName, MainCategory = (int)entity.MainCategory });
                sqlConnection.Close();
            }
        }

        public void Update(EventCategory entity)
        {
            var sql = "UPDATE [dbo].[EventCategory]"
                      + " SET [Name]=@Name,"
                      + " WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql,
                    new {entity.Id, entity.Name });
                sqlConnection.Close();
            }
        }

        public void Delete(int id)
        {
            var sql = "DELETE [dbo].[EventCategory] WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new {id});
                sqlConnection.Close();
            }
        }

        protected string GetHtmlName(string name, string tableName)
        {
            string html = name.StringToHtmlLink();
            var html1 = html;

            var htmlSql = $"SELECT [HtmlName] FROM [dbo].[{tableName}] WHERE [HtmlName] Like '{html}%'";
            IEnumerable<string> names;
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                names = sqlConnection.Query<string>(htmlSql);
                sqlConnection.Close();
            }

            var namesArray = names as string[] ?? names.ToArray();
            if (namesArray.Contains(html1))
            {
                int index = 2;
                var html2 = html;
                while (namesArray.Contains(html2 + "-" + index))
                {
                    index++;
                }
                html = html + "-" + index;
            }

            return html;
        }
    }
}
