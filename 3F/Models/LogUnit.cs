using System;

namespace _3F.Web.Models
{
    public class LogUnit
    {
        public string DateTime { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }

        public LogUnit(string[] items)
        {
            this.DateTime = items[0];
            this.Level = items[1];
            this.Message = items[2];
            this.Action = items.Length > 3 ? items[3] : "";
        }

        public LogUnit(string line)
            : this(line.Split(new string[] { ";" }, StringSplitOptions.None))
        { }
    }
}