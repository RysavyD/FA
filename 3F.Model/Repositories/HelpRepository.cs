using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using _3F.BusinessEntities;

namespace _3F.Model.Repositories
{
    public class HelpRepository : IHelpRepository
    {
        public IEnumerable<Help> GetAll()
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var helpEntities = sqlConnection.Query<Help>("SELECT [Id], [Question], [Answer] FROM [dbo].[Help]");
                sqlConnection.Close();
                return helpEntities;
            }
        }

        public Help GetById(int id)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var helpEntity =
                    sqlConnection.QueryFirst<Help>("SELECT [Id], [Question], [Answer] FROM [dbo].[Help] WHERE ID=@id",
                        new {id});
                sqlConnection.Close();
                return helpEntity;
            }
        }

        public void Add(Help item)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "INSERT INTO [dbo].[Help] ([Question], [Answer]) VALUES (@question, @answer)",
                    new {question = item.Question, answer = item.Answer});
                sqlConnection.Close();
            }
        }

        public void Update(Help item)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "UPDATE [dbo].[Help] SET [Question]=@question, [Answer]=@answer WHERE [Id]=@id",
                    new {question = item.Question, answer = item.Answer, id = item.Id});
                sqlConnection.Close();
            }
        }

        public void Delete(int id)
        {
            var sql = "DELETE [dbo].[Help] WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new { id });
                sqlConnection.Close();
            }
        }
    }

    public interface IHelpRepository : IEntityRepository<Help>
    { }
}
