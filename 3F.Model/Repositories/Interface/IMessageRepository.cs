using Dapper;
using System;
using System.Data.SqlClient;

namespace _3F.Model.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        public int UnreadedMessages(int userId)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var messageCount = sqlConnection.ExecuteScalar("SELECT COUNT(*) FROM [dbo].[MessageRecipient]"
                    + "  WHERE[Id_User] = @userId AND[Unreaded] = 1 AND[Visible] = 1 ", new { userId });
                sqlConnection.Close();
                return Convert.ToInt32(messageCount);
            }
        }
    }

    public interface IMessageRepository
    {
        int UnreadedMessages(int userId);
    }
}
