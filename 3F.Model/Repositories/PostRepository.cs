using System.Collections.Generic;
using System.Data.SqlClient;
using _3F.BusinessEntities;
using Dapper;

namespace _3F.Model.Repositories
{
    public class PostRepository : IPostRepository
    {
        public IEnumerable<Post> GetAll()
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var postEntities = sqlConnection.Query<Post>("SELECT [Id], [Name], [HtmlName], [Content], EditPermissions, ViewPermissions, OriginalUrl, [Icon] FROM [dbo].[Post]");
                sqlConnection.Close();
                return postEntities;
            }
        }

        public Post GetById(int id)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var postEntity =
                    sqlConnection.QueryFirst<Post>("SELECT [Id], [Name], [HtmlName], [Content], EditPermissions, ViewPermissions, OriginalUrl, [Icon] FROM [dbo].[Post] WHERE [Id]=@id",
                        new { id });
                sqlConnection.Close();
                return postEntity;
            }
        }

        public void Add(Post item)
        {
            var editPermissions = string.IsNullOrEmpty(item.EditPermissions) ? string.Empty : item.EditPermissions;
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "INSERT INTO [dbo].[Post] ([Name], [HtmlName], [Content], EditPermissions, ViewPermissions, OriginalUrl, [Icon]) VALUES (@name, @htmlName, @content, @editPermissions, @viewPermissions, @originalUrl, @icon)",
                    new { name = item.Name, htmlName = item.HtmlName, content = item.Content, editPermissions, viewPermissions = item.ViewPermissions, originalUrl = item.OriginalUrl, icon = item.Icon });
                sqlConnection.Close();
            }
        }

        public void Update(Post item)
        {
            var editPermissions = string.IsNullOrEmpty(item.EditPermissions) ? string.Empty : item.EditPermissions;
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(
                    "UPDATE [dbo].[Post] SET [Name]=@name, [HtmlName]=@htmlName, [Content]=@content, [EditPermissions]=@editPermissions, [ViewPermissions]=@viewPermissions, [OriginalUrl]=@originalUrl, [Icon]=@icon WHERE [Id]=@id",
                    new { id = item.Id, name = item.Name, htmlName = item.HtmlName, content = item.Content, editPermissions, viewPermissions = item.ViewPermissions, originalUrl = item.OriginalUrl, icon = item.Icon });
                sqlConnection.Close();
            }
        }

        public void Delete(int id)
        {
            var sql = "DELETE [dbo].[Post] WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new { id });
                sqlConnection.Close();
            }
        }

        public Post GetByHtml(string html)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var postEntity =
                    sqlConnection.QueryFirst<Post>("SELECT [Id], [Name], [HtmlName], [Content], EditPermissions, ViewPermissions, OriginalUrl, [Icon] FROM [dbo].[Post] WHERE [HtmlName]=@html",
                        new { html });
                sqlConnection.Close();
                return postEntity;
            }
        }
    }
}
