using System;
using System.Data.Entity.Spatial;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using _3F.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Web.Models;
using _3F.Web.Models.Collector;

namespace _3F.Web.Controllers
{
    public abstract class BaseCollectorController<TItemEntity, TItemOwnerEntity> : BaseController 
        where TItemEntity : class, IPrimaryKey, ICollectorItem
        where TItemOwnerEntity : class, IPrimaryKey, ICollectorOwner
    {
        protected string ControllerName = string.Empty;
        protected IDownload Import;

        #region Index
        public virtual ActionResult Index()
        {
            var owners = GetOwners();
            var topItems = owners.Where(o => o.Status == ItemOwnerStatus.Have).GroupBy(o => o.Id_Item).OrderByDescending(o => o.Count()).Take(3);
            var mostEntity = repository.One<TItemEntity>(e => e.Id == topItems.First().Key);

            var model = new CollectorIndexViewModel()
            {
                Title = ControllerName + " - Statistika",
                Count = entities.Count(),
                DifferentCount = owners
                    .Where(o => o.Status == ItemOwnerStatus.Have)
                    .Select(e => e.Id_Item)
                    .Distinct()
                    .Count(),
                MostItemName = mostEntity.Name,
                MostItemId = mostEntity.ItemNumber,
                MostItemIdCount = owners.Count(o => o.Status == ItemOwnerStatus.Have && o.Id_Item == mostEntity.Id),
            };

            SetLongestRow(owners, model);
            SetTopUsers(owners, model);
            SetTopHaveItems(owners, model);
            SetTopWantItems(owners, model);

            AddButtons(model, GetBaseUrl(""));
            return View("~/Views/Shared/BaseCollector/Index.cshtml", model);
        }

        private void SetLongestRow(IQueryable<Owner> owners, CollectorIndexViewModel model)
        {
            var haveOwners = owners
                .Where(o => o.Status == ItemOwnerStatus.Have)
                .GroupBy(o => o.AspNetUsers);

            int longestCounter = 0;
            AspNetUsers longestUser = null;
            foreach (var owner in haveOwners)
            {
                int longcount = 0;
                int count = 1;
                int pivot = 0;
                var arrayIds = owner
                    .Select(o => o.Id_Item)
                    .OrderBy(o => o)
                    .ToArray();
                var array = repository.Where<TItemEntity>(t => arrayIds.Contains(t.Id))
                    .Select(t => t.ItemNumber)
                    .ToArray();

                while(pivot + count < array.Length)
                {
                    if (array[pivot + count] != array[pivot] + count)
                    {
                        if (longcount < count)
                            longcount = count;

                        pivot = pivot + count;
                        count = 0;
                    }
                    count++;
                }

                if (longcount > longestCounter)
                {
                    longestCounter = longcount;
                    longestUser = owner.Key;
                }
            }

            model.MostItemInRowUser = new User(longestUser);
            model.MostItemInRow = longestCounter;
        }

        private void SetTopUsers(IQueryable<Owner> owners, CollectorIndexViewModel model)
        {
            var topCollectors = owners
                .Where(o => o.Status == ItemOwnerStatus.Have)
                .GroupBy(o => o.AspNetUsers)
                .OrderByDescending(o => o.Count())
                .Take(3)
                .ToArray();

            if (topCollectors.Length > 0)
            {
                model.Top1User = new User(topCollectors[0].Key);
                model.Top1UserCount = topCollectors[0].Count();
            }

            if (topCollectors.Length > 1)
            {
                model.Top2User = new User(topCollectors[1].Key);
                model.Top2UserCount = topCollectors[1].Count();
            }

            if (topCollectors.Length > 2)
            {
                model.Top3User = new User(topCollectors[2].Key);
                model.Top3UserCount = topCollectors[2].Count();
            }
        }

        private void SetTopHaveItems(IQueryable<Owner> owners, CollectorIndexViewModel model)
        {
            var topItems = owners
                .Where(o => o.Status == ItemOwnerStatus.Have)
                .GroupBy(o => o.Id_Item)
                .OrderByDescending(o => o.Count())
                .ThenBy(o => o.Key)
                .Take(3)
                .ToArray();

            if (topItems.Length > 0)
            {
                var item = repository.One<TItemEntity>(topItems[0].Key);
                model.Top1HaveItemId = item.ItemNumber;
                model.Top1HaveItemName = item.Name;
                model.Top1HaveItemCount = owners.Count(o => o.Status == ItemOwnerStatus.Have && o.Id_Item == item.Id);
            }

            if (topItems.Length > 1)
            {
                var item = repository.One<TItemEntity>(topItems[1].Key);
                model.Top2HaveItemId = item.ItemNumber;
                model.Top2HaveItemName = item.Name;
                model.Top2HaveItemCount = owners.Count(o => o.Status == ItemOwnerStatus.Have && o.Id_Item == item.Id);
            }

            if (topItems.Length > 2)
            {
                var item = repository.One<TItemEntity>(topItems[2].Key);
                model.Top3HaveItemId = item.ItemNumber;
                model.Top3HaveItemName = item.Name;
                model.Top3HaveItemCount = owners.Count(o => o.Status == ItemOwnerStatus.Have && o.Id_Item == item.Id);
            }
        }

        private void SetTopWantItems(IQueryable<Owner> owners, CollectorIndexViewModel model)
        {
            var topItems = owners
                .Where(o => o.Status == ItemOwnerStatus.WantTo)
                .GroupBy(o => o.Id_Item)
                .OrderByDescending(o => o.Count())
                .ThenBy(o => o.Key)
                .Take(3)
                .ToArray();

            if (topItems.Length > 0)
            {
                var item = repository.One<TItemEntity>(topItems[0].Key);
                model.Top1WantItemId = item.ItemNumber;
                model.Top1WantItemName = item.Name;
                model.Top1WantItemCount = owners.Count(o => o.Status == ItemOwnerStatus.WantTo && o.Id_Item == item.Id);
            }

            if (topItems.Length > 1)
            {
                var item = repository.One<TItemEntity>(topItems[1].Key);
                model.Top2WantItemId = item.ItemNumber;
                model.Top2WantItemName = item.Name;
                model.Top2WantItemCount = owners.Count(o => o.Status == ItemOwnerStatus.WantTo && o.Id_Item == item.Id);
            }

            if (topItems.Length > 2)
            {
                var item = repository.One<TItemEntity>(topItems[2].Key);
                model.Top3WantItemId = item.ItemNumber;
                model.Top3WantItemName = item.Name;
                model.Top3WantItemCount = owners.Count(o => o.Status == ItemOwnerStatus.WantTo && o.Id_Item == item.Id);
            }
        }
        #endregion

        #region Detail
        public ActionResult Detail(int id)
        {
            var entity = repository.One<TItemEntity>(e => e.ItemNumber == id);

            var model = new CollectorDetailViewModel()
            {
                Description = entity.Description,
                ItemId = entity.ItemNumber,
                ImageUrl = entity.ImageUrl,
                Name = entity.Name,
                Latitude = (entity.Position != null && entity.Position.Latitude.HasValue) ? entity.Position.Latitude.Value : 0d,
                Longitude = (entity.Position != null && entity.Position.Longitude.HasValue) ? entity.Position.Longitude.Value : 0d,
                Title = string.Format("{0} - {1}", entity.ItemNumber, entity.Name),
                BaseUrl = GetBaseUrl("/Detail/" + id),
             };

            model.NearestItems = entities.OrderBy(e => e.Position.Distance(entity.Position))
                .Skip(1)
                .Take(10)
                .Select(s => new CollertorItem()
                {
                    Name = s.Name,
                    ItemNumber = s.ItemNumber,
                    Distance = s.Position.Distance(entity.Position).Value,
                })
                .ToList();

            var owners = GetOwners();
            model.HaveUser = owners
                .Where(o => o.Id_Item == entity.Id && o.Status == ItemOwnerStatus.Have)
                .Select(o => new User()
                {
                    htmlName = o.AspNetUsers.HtmlName,
                    name = o.AspNetUsers.UserName,
                })
                .ToArray();

            model.VisitUser = owners
                .Where(o => o.Id_Item == entity.Id && o.Status == ItemOwnerStatus.Visited)
                .Select(o => new User()
                {
                    htmlName = o.AspNetUsers.HtmlName,
                    name = o.AspNetUsers.UserName,
                })
                .ToArray();

            model.WantUser = owners
                .Where(o => o.Id_Item == entity.Id && o.Status == ItemOwnerStatus.WantTo)
                .Select(o => new User()
                {
                    htmlName = o.AspNetUsers.HtmlName,
                    name = o.AspNetUsers.UserName,
                })
                .ToArray();

            AddButtons(model, model.BaseUrl);
            return View("~/Views/Shared/BaseCollector/Detail.cshtml", model);
        }

        [Authorize, HttpPost, ValidateAntiForgeryToken, ActionName("Detail")]
        public ActionResult DetailPost(int id, string type)
        {
            ItemOwnerStatus status;

            switch(type)
            {
                case "Vlastním předmět":
                    status = ItemOwnerStatus.Have;
                    break;
                case "Navštívil jsem, ale nemám":
                    status = ItemOwnerStatus.Visited;
                    break;
                case "Nevlastním předmět":
                    status = ItemOwnerStatus.NotHave;
                    break;
                case "Rád bych získal":
                    status = ItemOwnerStatus.WantTo;
                    break;
                default:
                    status = ItemOwnerStatus.NotHave;
                    break;
            }

            WriteOwner(status, id);

            return RedirectToAction("Detail", new { id });
        }
        #endregion

        #region Prehled
        public ActionResult Prehled()
        {
            var model = new CollectorItemsOverview()
            {
                Title = "Přehled",
                BaseUrl = GetBaseUrl("/Prehled"),
                Entities = entities.Select(e => new CollertorItem()
                {
                    Description = e.Description,
                    Id_Item = e.Id,
                    ItemNumber = e.ItemNumber,
                    Name = e.Name,
                })
                .OrderBy(e => e.ItemNumber)
                .ToArray(),
            };

            var owners = GetOwners().ToArray();

            foreach (var item in model.Entities)
            {
                item.Color = (User.Identity.IsAuthenticated) ? GetColor(item.Id_Item, owners) : "";
            }

            AddButtons(model, model.BaseUrl);

            return View("~/Views/Shared/BaseCollector/Prehled.cshtml", model);
        }

        protected string GetColor(int id_Item, Owner[] owners)
        {
            var owner = owners.FirstOrDefault(o => o.Id_Item == id_Item && o.AspNetUsers == GetUser);
            if (owner == null)
                return string.Empty;
            switch(owner.Status)
            {
                case ItemOwnerStatus.Have:
                    return "btn btn-success";
                case ItemOwnerStatus.Visited:
                    return "btn btn-warning";
                case ItemOwnerStatus.WantTo:
                    return "btn btn-info";
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Previous and Next
        public ActionResult Previous(int id)
        {
            int index = id;
            int? resultId = null;

            while(!resultId.HasValue)
            {
                index--;
                var item = repository.One<TItemEntity>(e => e.ItemNumber == index);
                if (index <= 1)
                    resultId = id;
                if (item != null)
                    resultId = index;
            }           

            return RedirectToAction("Detail", new { id = resultId });
        }

        public ActionResult Next(int id)
        {
            int index = id;
            int limit = entities.Max(e => e.ItemNumber);
            int? resultId = null;

            while (!resultId.HasValue)
            {
                index++;
                var item = repository.One<TItemEntity>(e => e.ItemNumber == index);
                if (index >= limit)
                    resultId = id;
                if (item != null)
                    resultId = index;
            }

            return RedirectToAction("Detail", new { id = resultId });
        }
        #endregion

        #region Import
        public ActionResult ImportItems(int? id, bool? overwrite, bool? singleItem, int? pageStart)
        {
            if (Import != null)
            {
                Import.Download(id.HasValue ? id.Value : 0, overwrite.HasValue ? overwrite.Value : false, singleItem.HasValue ? singleItem.Value : false, pageStart);

                CreateToastrMessage("Import proběhl v pořádku");
            }

            return View("~/Views/Shared/BaseCollector/Import.cshtml", new EmptyBaseViewModel() { Title = "Import" });
        }
        #endregion

        #region HandAdd
        [Authorize(Roles = Definitions.Strings.TouristAdmin)]
        public ActionResult HandAdd()
        {
            var model = new CollectorDetailViewModel() { Title = "Ruční přidání předmětu" };

            AddButtons(model, GetBaseUrl("/HandAdd"));
            return View("~/Views/Shared/BaseCollector/HandAdd.cshtml", model);
        }

        [Authorize(Roles = Definitions.Strings.TouristAdmin), HttpPost, ActionName("HandAdd"), ValidateAntiForgeryToken]
        public ActionResult HandAddPost(CollectorDetailViewModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                ModelState.AddModelError("", "Je třeba vyplnit jméno");

            if (model.ItemId <= 0)
                ModelState.AddModelError("", "Je třeba vyplnit číslo předmětu");

            if (ModelState.IsValid)
            {
                var entity = entities.FirstOrDefault(e => e.ItemNumber == model.ItemId);
                var longtitude = model.Longitude.ToString().Replace(",", ".");
                var latitude = model.Latitude.ToString().Replace(",", ".");

                if (entity == null)
                {
                    entity = Activator.CreateInstance<TItemEntity>();
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.ImageUrl = model.ImageUrl;
                    entity.ItemNumber = model.ItemId;
                    entity.Position= DbGeography.FromText(string.Format("POINT({0} {1})", longtitude, latitude));
                    repository.Add(entity);
                }
                else
                {
                    entity.Name = model.Name;
                    entity.Description = model.Description;
                    entity.ImageUrl = model.ImageUrl;
                    entity.ItemNumber = model.ItemId;
                    entity.Position = DbGeography.FromText(string.Format("POINT({0} {1})", longtitude, latitude));
                }

                repository.Save();

                CreateToastrMessage("Předmět " + model.Name + " byl úspěšně přidán.");
                return RedirectToAction("Prehled");
            }
            else
            {
                model.Title = "Ruční přidání předmětu";
                AddButtons(model, GetBaseUrl("/HandAdd"));

                return View("~/Views/Shared/BaseCollector/HandAdd.cshtml", model);
            }
        }
        #endregion

        #region Moje
        public ActionResult Moje()
        {
            var userId = GetUserId;
            var items = GetOwners()
                .Where(o => o.AspNetUsers.Id == userId)
                .OrderBy(o => o.Status)
                .ThenBy(o => o.Id_Item)
                .ToArray()
                .Select(o => new ItemWithOwner()
                {
                    ItemNumber = repository.One<TItemEntity>(e => e.Id == o.Id_Item).ItemNumber,
                    Status = o.Status.GetDescription(),
                    Name = repository.One<TItemEntity>(e => e.Id == o.Id_Item).Name
                })
                .ToArray();

            var model = new OwnerItems()
            {
                Title = GetUser.UserName,
                BaseUrl = GetBaseUrl("/Moje"),
                Entities = items,
            };

            AddButtons(model, model.BaseUrl);
            
            return View("~/Views/Shared/BaseCollector/Moje.cshtml", model);
        }
        #endregion

        #region Sberatele
        public ActionResult Sberatele()
        {
            var owners = GetOwners()
                .Select(o => o.AspNetUsers)
                .Distinct()
                .OrderBy(u => u.UserName)
                .Select(u => new OwnerRecord()
                {
                    User = new User()
                    {
                        name = u.UserName,
                        htmlName = u.HtmlName,
                    },
                })
                .ToArray();

            foreach(var owner in owners)
            {
                owner.HaveCount = GetOwners().Count(o => o.AspNetUsers.HtmlName == owner.User.htmlName && o.Status == ItemOwnerStatus.Have);
                owner.VisitedCount = GetOwners().Count(o => o.AspNetUsers.HtmlName == owner.User.htmlName && o.Status == ItemOwnerStatus.Visited);
                owner.WantCount = GetOwners().Count(o => o.AspNetUsers.HtmlName == owner.User.htmlName && o.Status == ItemOwnerStatus.WantTo);
            }

            var model = new OwnerRecords()
            {
                BaseUrl = GetBaseUrl("/Sberatele"),
                Title = "Sběratelé",
                Entities = owners,
            };

            AddButtons(model, model.BaseUrl);

            return View("~/Views/Shared/BaseCollector/Sberatele.cshtml", model);
        }
        #endregion

        #region Sberatel
        public ActionResult Sberatel(string id)
        {
            var user = repository.One<AspNetUsers>(u => u.HtmlName == id);
            int userId = user.Id;

            var items = GetOwners()
                .Where(o => o.AspNetUsers.Id == userId)
                .OrderBy(o => o.Status)
                .ThenBy(o => o.Id_Item)
                .ToArray()
                .Select(o => new ItemWithOwner()
                {
                    ItemNumber = repository.One<TItemEntity>(e => e.Id == o.Id_Item).ItemNumber,
                    Status = o.Status.GetDescription(),
                    Name = repository.One<TItemEntity>(e => e.Id == o.Id_Item).Name
                })
                .ToArray();

            var model = new OwnerItems()
            {
                Title = user.UserName,
                BaseUrl = GetBaseUrl("/Sberatel/" + id),
                Entities = items,
            };

            AddButtons(model, model.BaseUrl);

            return View("~/Views/Shared/BaseCollector/Moje.cshtml", model);
        }
        #endregion

        #region Export
        public ActionResult ExportToCsv()
        {
            var path = Path.GetTempPath();
            var fileName = string.Format("{0}_{1}.csv",
                GetUser.UserName,
                Info.CentralEuropeNow.ToString("yyyy-MM-dd"));

            var fullPath = Path.Combine(path, fileName);
            var ownerIds = repository.Where<TItemOwnerEntity>(to => to.Id_Owner == GetUserId && to.Status == ItemOwnerStatus.Have)
                .Select(to => to.Id_Item)
                .ToArray();

            var items =
                repository.Where<TItemEntity>(ie => ownerIds.Contains(ie.Id))
                    .Select(ie => ie.ItemNumber)
                    .OrderBy(num => num)
                    .Select(num => num.ToString())
                    .ToArray();

            System.IO.File.AppendAllLines(fullPath, items);

            return File(fullPath, "text/csv", fileName);
        }
        #endregion

        protected virtual void AddButtons(BaseViewModel model, string BaseUrl)
        {
            model.Buttons.Add(new ActionButton("Statistika", BaseUrl, "icon-bar-chart"));
            model.Buttons.Add(new ActionButton("Přehled předmětů", BaseUrl + "/Prehled", "icon-list-alt"));
            model.Buttons.Add(new ActionButton("Moje předměty", BaseUrl + "/Moje", "icon-eye-open"));
            model.Buttons.Add(new ActionButton("Sběratelé", BaseUrl + "/Sberatele", "icon-puzzle-piece"));
            model.Buttons.Add(new ActionButton("Export do csv", BaseUrl + "/ExportToCsv", "icon-file-text"));

            if (User.IsInRole(Definitions.Strings.TouristAdmin))
            {
                model.Buttons.Add(new ActionButton("Ruční přidání", BaseUrl + "/HandAdd", "icon-plus"));
            }
        }

        protected virtual IQueryable<Owner> GetOwners()
        {
            var owners = repository.All<TItemOwnerEntity>()
                .Select(o =>
                    new Owner()
                    {
                        Status = o.Status,
                        AspNetUsers = o.AspNetUsers,
                        Id_Item = o.Id_Item,
                    })
                .AsQueryable();

            return owners;
        }

        protected virtual void WriteOwner(ItemOwnerStatus status, int itemId)
        {
            var item = repository.One<TItemEntity>(ts => ts.ItemNumber == itemId);
            var entity = repository.One<TItemOwnerEntity>(o => o.Id_Item == item.Id && o.AspNetUsers == GetUser);
            if (entity == null)
            {
                entity = Activator.CreateInstance<TItemOwnerEntity>();

                entity.AspNetUsers = GetUser;
                entity.Id_Item = item.Id;
                entity.Status = status;

                repository.Add(entity);
            }
            else
            {
                entity.Status = status;
            }

            repository.Save();
        }

        protected IQueryable<TItemEntity> entities
        {
            get { return repository.All<TItemEntity>(); }
        }

        protected string GetBaseUrl(string controllerPath)
        {
            return string.IsNullOrWhiteSpace(controllerPath) ? Request.Url.OriginalString : Request.Url.OriginalString.Replace(controllerPath, "");
        }

        protected class Owner
        {
            public AspNetUsers AspNetUsers { get; set; }
            public ItemOwnerStatus Status { get; set; }
            public int Id_Item { get; set; }
        }
    }
}