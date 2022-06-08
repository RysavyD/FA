using System;

namespace _3F.Model.Repositories
{
    public interface IUserRepository
    {
        void UpdateUserActivity(int userId);
    }
}
