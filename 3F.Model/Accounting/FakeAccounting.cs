using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _3F.Model.Accounting
{
    public class FakeAccounting : IAccounting
    {
        Random rnd = new Random();

        public async Task<AccountData> GetData(string email)
        {
            if (rnd.NextDouble() > 0.5)
                return AccountData.EmptyData;
            else
                return new AccountData(email, 269, 66642, true);
        }

        public async Task<AccountData> GetData(int vs)
        {
            if (rnd.NextDouble() > 0.5)
                return AccountData.EmptyData;
            else
                return new AccountData("sqwert@seznam.cz", 269, 66642, true);
        }

        public async Task<MoveResult> MakeMove(int vs, double amount, string email, int event_vs, string note, int id_Payment)
        {
            if (rnd.NextDouble() > 0.33)
                return new MoveResult(true, true);

            if (rnd.NextDouble() > 0.5)
                return new MoveResult(false, true);

            return new MoveResult(false, false);
        }

        public async Task<NewUserResult> GetNewUserSymbol(string email, string userName)
        {
            if (rnd.NextDouble() > 0.5)
                return new NewUserResult("problem veliký", 0);
            else
                return new NewUserResult(string.Empty, rnd.Next(10000, 99999));
        }


        public async Task<List<Cost>> GetCosts(int eventNumber)
        {
            List<Cost> result = new List<Cost>();
            int count = rnd.Next(1, 10);
            for (int i = 0; i < count; i++)
            {
                result.Add(new Cost()
                {
                    Description = "Drahá faktura č." + i.ToString(),
                    Amount = rnd.Next(-1000, 1000),
                });
            }

            return result;
        }
    }
}
