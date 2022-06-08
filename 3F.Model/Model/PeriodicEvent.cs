namespace _3F.Model.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PeriodicEvent : IPrimaryKey
    {
        public int Id { get; set; }
        
        public PeriodicEventTypeEnum PeriodicEventType { get; set; }

        [Required]
        public string EventNameFormat { get; set; }

        public int PeriodicParameter { get; set; }
    }

    public enum PeriodicEventTypeEnum
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Yearly = 3,
        DayInWeek = 4,
        DayInMonth = 5,
    }
}
