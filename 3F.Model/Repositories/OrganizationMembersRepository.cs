using System.Data.SqlClient;
using Dapper;
using _3F.Model.Repositories.Interface;

namespace _3F.Model.Repositories
{
    public class OrganizationMembersRepository : IOrganizationMembersRepository
    {
        public bool IsMember(int userId)
        {
            var sql = "SELECT [Id] FROM [dbo].[OrganisationMember] WHERE Id = @id AND [To] IS NULL";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var entities = sqlConnection.QueryFirstOrDefault(sql, new {id = userId});
                sqlConnection.Close();
                return entities != null;
            }
        }
    }
}
