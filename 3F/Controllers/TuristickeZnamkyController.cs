using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Models;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class TuristickeZnamkyController : BaseCollectorController<TouristStamp, TouristStampOwner>
    {
        public TuristickeZnamkyController()
        {
            Icon = "icon-circle";
            BackgroundColor = "brown-background";
            ControllerName = "Turistické známky";
            Import = new TouristStampDownload(repository, logger);
        }

        public ActionResult Csv()
        {
            var model = new EmptyBaseViewModel() { Title = "Import dat z csv" };
            AddButtons(model, GetBaseUrl("/Csv"));

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Csv")]
        public ActionResult CsvPost(HttpPostedFileBase file)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    using (var reader = new StreamReader(file.InputStream))
                    {
                        reader.ReadLine(); //prvni radek je info o turistickych znamkach
                        while (!reader.EndOfStream)
                        {
                            int stampNumber;
                            if (int.TryParse(reader.ReadLine(), out stampNumber))
                            {
                                if (stampNumber != 0)
                                {
                                    var stamp = repository.One<TouristStamp>(ts => ts.ItemNumber == stampNumber);
                                    if (stamp == null)
                                        continue;

                                    var owner = repository.One<TouristStampOwner>(tso => tso.Id_Item == stamp.Id && tso.Id_Owner == GetUserId);
                                    if (owner != null)
                                        owner.Status = ItemOwnerStatus.Have;
                                    else
                                        repository.Add(new TouristStampOwner()
                                        {
                                            AspNetUsers = GetUser,
                                            Status = ItemOwnerStatus.Have,
                                            TouristStamp = stamp,
                                        });
                                }
                            }
                        }
                        repository.Save();
                    }

                    CreateToastrMessage("Import úspěšně proběhl.");
                }
                else
                {
                    CreateToastrMessage("Chyba souboru.");
                }
            }
            catch(Exception ex)
            {
                CreateToastrMessage("Při importu nastala chyba.");
                logger.LogException(ex, "TuristickeZnamky.Csv");
            }

            return RedirectToAction("Moje");
        }

        public ActionResult Mapa(int? start, int? stop)
        {
            int downLimit = start ?? 0;
            int upLimit = stop ?? 3000;

            var model = repository.Where<TouristStamp>(s => downLimit <= s.ItemNumber && s.ItemNumber <= upLimit)
                .ToArray()
                .Select(s => new TuristickeZnamkyModel()
                {
                    ItemNumber = s.ItemNumber,
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    AlreadyHaven =
                        s.TouristStampOwner.Any(o => o.AspNetUsers == GetUser && o.Status == ItemOwnerStatus.Have),
                    Coordinates =  $"{s.Position.Longitude.ToString().Replace(",",".")}, {s.Position.Latitude.ToString().Replace(",", ".")}",
                })
                .ToArray();

            return View(model);
        }



        protected override void AddButtons(BaseViewModel model, string baseUrl)
        {
            base.AddButtons(model, baseUrl);

            model.Buttons.Add(new ActionButton("Import z csv", baseUrl + "/Csv", "icon-file-text"));
        }
    }
}