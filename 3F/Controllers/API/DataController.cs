using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Controllers.API.Model;
using System.Linq;
using System.Web.Http;


namespace _3F.Web.Controllers.API
{
    public class DataController : BaseApiController
    {
        public DataController(IRepository repository)
            : base (repository)
        {
        }

        [HttpGet]
        public IHttpActionResult Events()
        {
            var events = repository.Where<Event>(ev => ev.State == EventStateEnum.Active);
            var result = events
                .Select(ev => new EventApiModel()
                {
                    Name = ev.Name,
                    Price = ev.Price,
                    ActionId = ev.AccountSymbol,
                    StartTime = ev.StartDateTime,
                    StopTime = ev.StopDateTime,
                    Organisators = ev.EventOrganisator.Select(o => new OrganisatorApiModel()
                    {
                        Name = o.AspNetUsers.UserName
                    }),
                })
                .ToArray();

            return Ok(result);
        }

        [HttpGet]
        public IHttpActionResult AccountancyInfo()
        {
            var actualEvents = repository.Where<Event>(ev => ev.StartDateTime > Info.CentralEuropeNow && ev.State == EventStateEnum.Active
                && ev.EventType == EventTypeEnum.PlacenaSdruzenim);

            var participantsPaid = actualEvents
                .SelectMany(ev => ev.EventParticipant.Where(p => p.EventLoginStatus == EventLoginEnum.Prijdu))
                .ToArray();
            var participantsReservation = actualEvents
                .SelectMany(ev => ev.EventParticipant.Where(p => p.EventLoginStatus == EventLoginEnum.Rezervace))
                .ToArray();
            
            var sumPaid = participantsPaid.Sum(p => p.Event.Price);
            var sumReservation = participantsReservation.Sum(p => p.Event.Price);

            var info = new AccountancyInfoApiModel()
            {
                PocetPredplacenychAkci = actualEvents.Count(),
                PocetUcastniku = participantsPaid.Count() + participantsReservation.Count(),
                OdhadovaneVynosy = sumPaid + sumReservation,
                VRezervaci = sumReservation,
                JizZaplaceno = sumPaid,
            };

            return Ok(info);
        }        
    }
}
