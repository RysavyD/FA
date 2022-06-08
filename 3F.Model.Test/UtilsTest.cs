using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3F.Model.Test
{
    [TestClass]
    public class UtilsTest
    {
        [TestMethod]
        public void Svatek()
        {
            string name = Utils.Svatek.GetSvatek(new DateTime(2015, 4, 9));

            Assert.AreSame(name, "Dušan");

            var today = Info.CentralEuropeNow;
            name = Utils.Svatek.GetSvatek(today);
            string todayName = Utils.Svatek.DnesniSvatek();
            Assert.AreSame(name, todayName);
        }
    }
}
