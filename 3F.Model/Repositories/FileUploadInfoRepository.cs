using System.Collections.Generic;
using System.Data.SqlClient;
using _3F.BusinessEntities;
using Dapper;

namespace _3F.Model.Repositories
{
    public class FileUploadInfoRepository : IFileUploadInfoRepository
    {
        public IEnumerable<FileUploadInfo> GetFiles()
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var entities = sqlConnection.Query<FileUploadInfo>("SELECT [Id], [Name], [Path], [CreationDate], [Description] FROM [dbo].[FileUploadInfo]");
                sqlConnection.Close();
                return entities;
            }
        }

        public FileUploadInfo GetById(int id)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                var postEntity =
                    sqlConnection.QueryFirst<FileUploadInfo>("SELECT [Id], [Name], [Path], [CreationDate], [Description] FROM [dbo].[FileUploadInfo] WHERE [Id]=@id",
                        new { id });
                sqlConnection.Close();
                return postEntity;
            }
        }

        public void Add(FileUploadInfo item)
        {
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            { 
                sqlConnection.Open();
                sqlConnection.Execute(
                    "INSERT INTO [dbo].[FileUploadInfo] ([Name], [Path], [CreationDate], [Description]) VALUES (@name, @path, @creationDate, @description)",
                    new { name = item.Name, path = item.Path, creationDate = item.CreationDate, description = item.Description });
                sqlConnection.Close();
            }
        }

        public void Remove(int id)
        {
            var sql = "DELETE [dbo].[FileUploadInfo] WHERE [Id]=@Id";
            using (var sqlConnection = new SqlConnection(Info.ConnectionString))
            {
                sqlConnection.Open();
                sqlConnection.Execute(sql, new { id });
                sqlConnection.Close();
            }
        }
    }
}
