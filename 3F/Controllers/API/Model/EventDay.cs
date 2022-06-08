using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3F.Web.Controllers.API.Model
{
    public class EventDay
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }

        public EventDay(DateTime datetime)
        {
            this.Year = datetime.Year;
            this.Month = datetime.Month - 1;
            this.Day = datetime.Day;
        }
    }
}