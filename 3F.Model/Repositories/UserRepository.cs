using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace _3F.Model.Repositories
{
    public class UserRepository : IUserRepository
    {
        public void UpdateUserActivity(int userId)
        {
            Task.Run(() =>
            {
                using (var sqlConnection = new SqlConnection(Info.ConnectionString))
                {
                    sqlConnection.Open();

                    sqlConnection.Execute(
                        "update [AspNetUsers] set DateLastActivity = @currentTime where Id = @userId",
                        new { currentTime = Info.CentralEuropeNow, userId });

                    sqlConnection.Close();
                }
            });
        }
    }
}
