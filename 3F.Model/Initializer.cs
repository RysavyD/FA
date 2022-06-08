using System.Data.Entity;
using System.Linq;

namespace _3F.Model
{
    public class Initializer
    {
        public static void Initialize()
        {
            var repo = new InitEntities();
        }

        public static void Migrate()
        {
            var repo = new Repository();
            var history = repo.All<Model.C__MigrationHistory>().ToArray();
        }
    }

    class InitEntities : DbContext
    {
        public InitEntities()
            : base("name=DefaultConnection")
        {
            Database.CreateIfNotExists();
        }
    }
}
