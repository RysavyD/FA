using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using _3F.BusinessEntities.Diskuze;
using B = _3F.BusinessEntities;
using _3F.Model.Repositories.Interface;

namespace _3F.Model.Repositories
{
    public class DiscussionRepository : IDiscussionRepository
    {
        public IEnumerable<B.Discussion> Discussions
        {
            get
            {
                var result = new List<B.Discussion>();
                using (var sqlConnection = new SqlConnection(Info.ConnectionString))
                {
                    sqlConnection.Open();
                    var sql = "SELECT d.[Id], d.[Name], d.[HtmlName]"
                        + ", [Perex], (SELECT COUNT(*) FROM[dbo].[DiscussionItem] WHERE Id_Discussion = d.Id) AS ItemsCount"
                        + ", [CreateDate], [LastUpdateDate]"
                        + ", u.Id AS Author_Id, u.UserName AS Author_UserName, u.htmlName AS Author_HtmlName"
                        + ", u.ProfilePhoto AS Author_ProfilePhoto, u.LoginType AS Author_LoginType"
                        + " FROM[dbo].[Discussion] d"
                        + " LEFT JOIN[dbo].[AspNetUsers] u on d.Id_Author = u.Id WHERE[IsAlone] = 1 "
                        + " ORDER BY d.[LastUpdateDate] DESC";

                    using (var reader = sqlConnection.ExecuteReader(sql))
                    {
                        while (reader.Read())
                        {
                            var entity = new B.Discussion()
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString(),
                                HtmlName = reader[2].ToString(),
                                Perex = reader[3].ToString(),
                                ItemsCount = Convert.ToInt32(reader[4]),
                                CreateDate = Convert.ToDateTime(reader[5]),
                                LastUpdateDate = Convert.ToDateTime(reader[6]),
                                Author = new B.User()
                                {
                                    Id = Convert.ToInt32(reader[7]),
                                    UserName = reader[8].ToString(),
                                    HtmlName = reader[9].ToString(),
                                    ProfilePhoto = reader[10].ToString(),
                                    LoginType = Convert.ToInt32(reader[11]),
                                }
                            };
                            result.Add(entity);
                        }
                    }

                    sqlConnection.Close();
                    return result;
                }
            }
        }

        public Tuple<int, IEnumerable<B.DiscussionItem>> GetDiscussionItems(int discussionId, int page = 1, int pagesize = 10)
        {
            int offset = (page - 1) * pagesize;
            var sql = "SELECT COUNT(*) FROM [dbo].[DiscussionItem] d"
                + " WHERE Id_Discussion = @discussionId"
                + " "
                + " SELECT [Text] ,[DateTime]"
                + " ,u.Id AS Author_Id, u.UserName AS Author_UserName, u.htmlName AS Author_HtmlName"
                + " ,u.ProfilePhoto AS Author_ProfilePhoto, u.LoginType AS Author_LoginType"
                + " FROM[dbo].[DiscussionItem] d JOIN[dbo].[AspNetUsers] u ON d.Id_Author = u.Id"
                + " WHERE Id_Discussion= @discussionId ORDER BY[DateTime] DESC"
                + " OFFSET @offset ROWS FETCH NEXT @pagesize ROWS ONLY";

            var result = new List<B.DiscussionItem>();
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                int count;

                sqlConnection.Open();
                using (var reader = sqlConnection.ExecuteReader(sql, new { offset, pagesize, discussionId }))
                {
                    reader.Read();
                    count = Convert.ToInt32(reader[0]);

                    reader.NextResult();
                    while (reader.Read())
                    {
                        var entity = new B.DiscussionItem()
                        {
                            Text = reader[0].ToString(),
                            DateTime = Convert.ToDateTime(reader[1]),
                            Author = new B.User()
                            {
                                Id = Convert.ToInt32(reader[2]),
                                UserName = reader[3].ToString(),
                                HtmlName = reader[4].ToString(),
                                ProfilePhoto = reader[5].ToString(),
                                LoginType = Convert.ToInt32(reader[6]),
                            }
                        };
                        result.Add(entity);
                    }
                }

                sqlConnection.Close();
                return new Tuple<int, IEnumerable<B.DiscussionItem>>(count, result);
            }
        }

        public IEnumerable<LastDiscussion> GetLastDiscsuDiscussions()
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                var sql = "SELECT TOP 5"
                    + " ev.Name AS EventName, ev.HtmlName AS EventHtml"
                    + " ,summ.Name AS SummaryName ,evs.HtmlName AS SummaryHtml"
                    + " ,pho.Id AS PhotoId, 'Album k akci ' + evp.Name AS PhotoEventName"
                    + " ,d.[Name] AS DiscussionName, d. [HtmlName] AS DiscussionHtml"
                    + " FROM [dbo].[Discussion]  d"
                    + " LEFT JOIN [Event] ev ON ev.Id_Discussion = d.Id"
                    + " LEFT JOIN [EventSummary] summ ON summ.Id_Discussion = d.Id"
                    + " LEFT JOIN [Event] evs ON evs.Id = summ.Id_Event"
                    + " LEFT JOIN [PhotoAlbum] pho ON pho.Id_Discussion = d.Id"
                    + " LEFT JOIN[Event] evp ON evp.Id = pho.Id_Event"
                    + " order by d.[LastUpdateDate] DESC";

                sqlConnection.Open();
                var entities = sqlConnection.Query<LastDiscussion>(sql);
                sqlConnection.Close();
                return entities;
            }
        }
    }
}
