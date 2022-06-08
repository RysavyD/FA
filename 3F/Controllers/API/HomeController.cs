using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Controllers.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace _3F.Web.Controllers.API
{
    public class HomeController : BaseApiController
    {
        public HomeController(IRepository repository)
            : base (repository)
        {
        }

        [HttpGet]
        public IHttpActionResult EventDates()
        {
            List<DateTime> dates = new List<DateTime>();
            foreach (var dbEvent in repository.Where<Event>(e => e.State == EventStateEnum.Active && e.StartDateTime > Info.CentralEuropeNow))
            {
                if ((dbEvent.StopDateTime - dbEvent.StartDateTime).Days <= 7)
                {
                    //nezobrazovat dlouhodobe akce
                    dates.AddRange(Enumerable.Range(0, 1 + dbEvent.StopDateTime.Date.Subtract(dbEvent.StartDateTime.Date).Days)
                        .Select(i => dbEvent.StartDateTime.AddDays(i)));
                }
            }

            var result = dates.Distinct().Select(d => new EventDay(d));
            return Ok(result);
        }
    }
}
