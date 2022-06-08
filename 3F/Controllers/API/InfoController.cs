using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;
using _3F.Model;
using _3F.Model.Model;

namespace _3F.Web.Controllers.API
{
    public class InfoController : BaseApiController
    {
        public InfoController(IRepository repository) : base(repository)
        { }

        [HttpGet]
        public IHttpActionResult Events()
        {
            if (!Request.Headers.Accept.Any(a => a.MediaType == "application/xml"))
                Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var actions = repository.Where<Event>(ev => ev.State == EventStateEnum.Active && ev.EventType != EventTypeEnum.Soukroma);

            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode eventsNode = doc.CreateElement("events");

            foreach (var eventEntity in actions)
            {
                XmlNode eventNode = doc.CreateElement("event");

                XmlNode nameNode = doc.CreateElement("name");
                nameNode.InnerText = eventEntity.Name;
                eventNode.AppendChild(nameNode);

                XmlNode priceNode = doc.CreateElement("price");
                priceNode.InnerText = eventEntity.Price.ToString();
                eventNode.AppendChild(priceNode);

                XmlAttribute symbolAttribute = doc.CreateAttribute("number");
                symbolAttribute.Value = eventEntity.AccountSymbol.ToString();
                eventNode.Attributes.Append(symbolAttribute);

                XmlNode organisatorsNode = doc.CreateElement("Organisators");
                foreach (var organisator in eventEntity.EventOrganisator)
                {
                    XmlNode organisatorNode = doc.CreateElement("organisator");
                    organisatorNode.InnerText = organisator.AspNetUsers.UserName;

                    organisatorsNode.AppendChild(organisatorNode);
                }
                eventNode.AppendChild(organisatorsNode);

                XmlNode StartNode = doc.CreateElement("StartTime");
                StartNode.InnerText = eventEntity.StartDateTime.ToString("yyyy-MM-dd HH:mm");

                XmlNode StopNode = doc.CreateElement("StopTime");
                StopNode.InnerText = eventEntity.StopDateTime.ToString("yyyy-MM-dd HH:mm");

                eventNode.AppendChild(StartNode);
                eventNode.AppendChild(StopNode);

                eventsNode.AppendChild(eventNode);
            }

            doc.AppendChild(eventsNode);


            return Ok(doc);
        }

        [HttpGet]
        public IHttpActionResult AccountancyInfo()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            var events = repository.Where<Event>(ev => ev.State == EventStateEnum.Active && ev.EventType == EventTypeEnum.PlacenaSdruzenim && ev.StopDateTime >= Info.CentralEuropeNow);
            var participantsPlaceno = events
                .SelectMany(ev => ev.EventParticipant
                    .Where(ep => ep.EventLoginStatus == EventLoginEnum.Prijdu));

            var participantsRezervace = events
                .SelectMany(ev => ev.EventParticipant
                    .Where(ep => ep.EventLoginStatus == EventLoginEnum.Rezervace));

            var vybrano = participantsPlaceno.Any() ? participantsPlaceno.Sum(p => p.Event.Price) : 0;
            var vRezevaci = participantsRezervace.Any() ? participantsRezervace.Sum(p => p.Event.Price) : 0;


            XmlNode infosNode = doc.CreateElement("infos");

            AddNewNode(doc, "PocetPredplacenychAkci", events.Count(), infosNode);
            AddNewNode(doc, "PocetUcastniku", participantsPlaceno.Count() + participantsRezervace.Count(), infosNode);
            AddNewNode(doc, "OdhadovaneVynosy", vybrano + vRezevaci, infosNode);
            AddNewNode(doc, "VRezervaci", vRezevaci, infosNode);
            AddNewNode(doc, "JizZaplaceno", vybrano, infosNode);

            doc.AppendChild(infosNode);

            return Ok(doc);
        }

        private void AddNewNode(XmlDocument doc, string nodeName, object nodeValue, XmlNode parentNode)
        {
            XmlNode newNode = doc.CreateElement(nodeName);
            newNode.InnerText = nodeValue.ToString();
            parentNode.AppendChild(newNode);
        }
    }
}
