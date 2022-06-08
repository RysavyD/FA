using System;

namespace _3F.BusinessEntities.Akce
{
    public class Participant 
    {
        public int LoginStatus { get; set; }
        public bool IsExternal { get; set; }
        public DateTime Time { get; set; }
        public User User { get; set; }
    }
}
