using _3F.BusinessEntities;

namespace _3F.Model.Repositories
{
    public interface IPostRepository : IEntityRepository<Post>
    {
        Post GetByHtml(string html);
    }
}
