using System;

namespace _3F.Model.Accounting
{
    public class AccountData
    {
        public string Email { get; private set; }
        public int VS { get; private set; }
        public double Advance { get; private set; }
        public bool CommunicationOk { get; private set; }

        public AccountData(string Email, int VS, double Advance, bool CommunicationOk) : this(Email, VS.ToString(), Advance.ToString(), CommunicationOk) { }

        public AccountData(string Email, string VS, string Advance, bool CommunicationOk)
        {
            this.Email = Email;
            this.VS = Convert.ToInt32(VS);
            string ds = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            this.Advance = Convert.ToDouble(Advance.Replace(".", ds).Replace(",", ds));
            this.CommunicationOk = CommunicationOk;
        }

        public static AccountData EmptyData
        {
            get
            {
                return new AccountData(string.Empty, 0, 0, false);
            }
        }
    }

    public class MoveResult
    {
        public bool Paid { get; private set; }
        public bool CommunicationOk { get; private set; }

        public MoveResult(bool Paid, bool CommunicationOk)
        {
            this.Paid = Paid;
            this.CommunicationOk = CommunicationOk;
        }

        public static MoveResult EmptyResult
        {
            get
            {
                return new MoveResult(false, false);
            }
        }
    }

    public class NewUserResult
    {
        public string Problem { get; private set; }
        public int Symbol { get; private set; }

        public NewUserResult(string problem, int symbol)
        {
            this.Problem = problem;
            this.Symbol = symbol;
        }

        public static NewUserResult EmptyResult
        {
            get
            {
                return new NewUserResult(string.Empty, 0);
            }
        }
    }

    public class Cost
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
