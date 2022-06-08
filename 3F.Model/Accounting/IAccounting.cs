using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3F.Model.Accounting
{
    public interface IAccounting
    {
        Task<AccountData> GetData(string email);
        Task<AccountData> GetData(int vs);
        Task<MoveResult> MakeMove(int vs, double amount, string email, int event_vs, string note, int id_Payment);
        Task<NewUserResult> GetNewUserSymbol(string email, string userName);
        Task<List<Cost>> GetCosts(int eventNumber);
    }
}
