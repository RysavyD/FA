using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3F.Web.Models
{
    public class Values
    {
        private static Values instance;

        public static Values Instance
        {
            get
            {
                if (instance == null)
                    instance = new Values();

                return instance;
            }
        }

        public string AppDataPath { get; private set; }

        private Values()
        {
            AppDataPath = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data");
        }
    }
}