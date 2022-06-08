using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using _3F.BusinessEntities.Account;

namespace _3F.Model.Repositories
{
    public class KnowFromRepository : IKnowFromRepository
    {
        public IEnumerable<KnowFrom> GetAll()
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var entities = sqlConnection.Query<KnowFrom>("SELECT [Id], [Text], [Visible] FROM [dbo].[KnowFrom]");
                sqlConnection.Close();
                return entities;
            }
        }

        public KnowFrom GetById(int id)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var helpEntity =
                    sqlConnection.QueryFirst<KnowFrom>("SELECT [Id], [Text], [Visible] FROM [dbo].[KnowFrom] WHERE ID=@id",
                        new { id });
                sqlConnection.Close();
                return helpEntity;
            }
        }

        public void Add(KnowFrom item)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "INSERT INTO [dbo].[KnowFrom] ([Text], [Visible]) VALUES (@text, @visible)",
                    new { text = item.Text, visible = item.Visible });
                sqlConnection.Close();
            }
        }

        public void Update(KnowFrom item)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "UPDATE [dbo].[KnowFrom] SET [Text]=@text, [Visible]=@visible WHERE [Id]=@id",
                    new { text = item.Text, visible = item.Visible, id = item.Id });
                sqlConnection.Close();
            }
        }

        public void Delete(int id)
        {
            var sql = "DELETE [dbo].[KnowFrom] WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new { id });
                sqlConnection.Close();
            }
        }
    }
}
