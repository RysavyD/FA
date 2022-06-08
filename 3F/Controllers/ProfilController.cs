using System;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using _3F.Model;
using _3F.Model.Accounting;
using _3F.Model.Model;
using _3F.Model.Utils;
using _3F.Web.Definitions;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Models.Profil;
using _3F.Web.Utils;
using _3F.BusinessEntities.Enum;
using _3F.Model.Extensions;

namespace _3F.Web.Controllers
{
    [Authorize]
    public class ProfilController : BaseController
    {
        IAccounting accounting;
        ApplicationUserManager userManager;
        IAuthenticationManager authenticationManager;
        ApplicationSignInManager signInManager;

        public ProfilController(IAccounting accounting, ApplicationUserManager userManager, IAuthenticationManager authenticationManager, ApplicationSignInManager signInManager)
        {
            this.accounting = accounting;
            this.userManager = userManager;
            this.authenticationManager = authenticationManager;
            this.signInManager = signInManager;
            BackgroundColor = "blue-background";
        }

        #region Detail
        [AllowAnonymous]
        public async Task<ActionResult> Detail(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Detail", new { id = GetUserHtmlName });
                }

                return RedirectToAction("", "");
            }

            var user = repository.One<AspNetUsers>(u => u.HtmlName == id);
            string money = "Nepřiřazen VS";

            if (user == null)
                return HttpNotFound();

            var model = new ProfilViewModel()
            {
                Title = user.UserName,
                Icon = "icon-user",

                BirhtYear = user.Profiles.BirhtYear,
                CanEdit = user.UserName == User.Identity.Name,
                City = user.Profiles.City,
                RegistrationDate = user.DateCreated.ToString("dd. MM. yyyy"),
                Email = user.Email,
                Hobbies = user.Profiles.Hobbies,
                Image = user.ProfilePhoto.Replace("_prof.jpg", ".jpg"),
                Link = user.Profiles.Link,
                Money = money,
                Motto = user.Profiles.Motto,
                Name = user.Profiles.Name,
                HtmlName = user.HtmlName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Sex = user.Profiles.Sex,
                VariableSymbol = (user.VariableSymbol.HasValue) ? user.VariableSymbol.Value.ToString() : "Nepřiřazen",
                IsOrganisationMember = user.OrganisationMember.Any(ou => ou.Organisation.Name == Strings.Organisation && !ou.To.HasValue),
            };

            if (User.Identity.IsAuthenticated)
            {
                model.Status = user.Profiles.Status;

                if (model.CanEdit)
                {
                    var userLogins = await userManager.GetLoginsAsync(user.Id);
                    model.CurrentLogins = userLogins;
                    model.OtherLogins = authenticationManager.GetExternalAuthenticationTypes()
                        .Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();

                    if (user.VariableSymbol.HasValue)
                    {
                        var data = await accounting.GetData(user.VariableSymbol.Value);
                        model.Money = (data.CommunicationOk) ? string.Format("{0:c}", data.Advance) : "Nepodařilo se zjistit";
                    }
                }

                model.EventOrganisedCount = repository.Where<Event>(e => e.EventOrganisator.Any(o => o.Id_User == user.Id))
                    .OrderByDescending(e => e.StartDateTime).Count();

                model.EventMissedCount = repository.Where<EventParticipant>(p => p.EventLoginStatus == EventLoginEnum.Neomluven && p.Id_User == user.Id)
                    .Count();

                model.HistoryEvents = repository.Where<Event>(e => e.State == EventStateEnum.Active && e.StartDateTime < Info.CentralEuropeNow)
                    .Where(ev => ev.EventType != EventTypeEnum.Soukroma)
                    .Where(e => e.EventParticipant.Any(p => p.Id_User == user.Id && p.EventLoginStatus == EventLoginEnum.Prijdu && !p.IsExternal))
                    .OrderByDescending(e => e.StartDateTime).ToSimpleViewModel().ToList();

                model.OrganisedEvents = repository.Where<Event>(e => e.State == EventStateEnum.Active && e.StartDateTime < Info.CentralEuropeNow)
                    .Where(ev => ev.EventType != EventTypeEnum.Soukroma)
                    .Where(e => e.EventOrganisator.Any(o => o.Id_User == user.Id))
                    .OrderByDescending(e => e.StartDateTime).ToSimpleViewModel().ToList();

                var loginStatuses = new[]
                {
                    EventLoginEnum.Prijdu,
                    EventLoginEnum.Mozna,
                    EventLoginEnum.Nahradnik,
                    EventLoginEnum.NepotvrzenaRezervace,
                    EventLoginEnum.Rezervace,
                    EventLoginEnum.RezervacePropadla
                };

                model.FutureEvents = repository.Where<Event>(e => e.State == EventStateEnum.Active && e.StartDateTime > Info.CentralEuropeNow)
                        .Where(e => e.EventParticipant.Any(p => p.Id_User == user.Id && loginStatuses.Contains(p.EventLoginStatus) && !p.IsExternal))
                        .OrderBy(e => e.StartDateTime).ToSimpleViewModel().ToList();

                if (User.Identity.Name != user.UserName && !string.IsNullOrWhiteSpace(User.Identity.Name))
                {
                    logger.LogInfo(string.Format("Uživatel {0} navštívil profil uživatele {1}", User.Identity.Name, user.UserName), "Profil.Detail");

                    model.Buttons.Add(CreateActionButton("Poslat zprávu", "~/Zpravy/Vytvorit/" + user.HtmlName, "icon-envelope-alt"));
                    model.Buttons.Add(CreateActionButton("Vzájemné zprávy", "~/Zpravy/Vzajemne/" + user.HtmlName, "icon-exchange"));

                    if (User.IsInRole(Strings.Administrator))
                    {
                        model.Buttons.Add(CreateActionButton("Administrace uživatele", "~/Administrace/Uzivatel/" + user.HtmlName, "icon-cogs"));
                    }
                }
                else
                {
                    AddActionButtons(model);
                }
            }

            return View(model);
        }
        #endregion

        #region Password change
        public ActionResult Password()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Password")]
        public async Task<ActionResult> PasswordPost(ChangePassword model)
        {
            string success = "Heslo bylo úspěšně změněno";
            if (ModelState.IsValid)
            {
                var user = repository.One<AspNetUsers>(u => u.UserName == User.Identity.Name);

                if (user.LoginType == LoginTypeEnum.OldSystemConfirmed)
                {
                    var oldUser = repository.Where<OldPassword>(o => o.Id_User == user.Id).First();
                    if (Utilities.Validate(oldUser, model.OldPassword))
                    {
                        string resetToken = await userManager.GeneratePasswordResetTokenAsync(user.Id);
                        var result = await userManager.ResetPasswordAsync(user.Id, resetToken, model.Password);
                        if (result == IdentityResult.Success)
                        {
                            user.LoginType = LoginTypeEnum.Confirmed;
                            CreateToastrMessage(success);
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                                ModelState.AddModelError("", error);
                        }
                    }
                    else
                        ModelState.AddModelError("OldPassword", "Špatné staré heslo"); 
                }
                else if (user.LoginType == LoginTypeEnum.Confirmed)
                {
                    var result = await userManager.ChangePasswordAsync(user.Id, model.OldPassword, model.Password);
                    if (result == IdentityResult.Success)
                    {
                        CreateToastrMessage(success);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError("", error);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Účtu nejde změnit heslo");
                }
            }

            repository.Save();
            return View(model);
        }
        #endregion

        #region Photo
        public ActionResult Fotka()
        {
            var model = new ProfilPhotoViewModel();
            AddActionButtons(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Fotka")]
        public ActionResult FotkaPost(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                logger.LogDebug(string.Format("Uživatel {0} nahrává fotku {1} o velikosti {2}", User.Identity.Name,
                    file.FileName, file.ContentLength), "Profil.FotkaPost");

                var htmlName = GetUserHtmlName;
                string bigImagePath = Server.MapPath("~/Images/Profile/") + htmlName + ".jpg";
                string smallImagePath = Server.MapPath("~/Images/Profile/") + htmlName + "_prof.jpg";

                Image image = null;
                Image smallImage = null;
                try
                {
                    image = Image.FromStream(file.InputStream);
                    smallImage = ResizeImage.ResizeImageIfNeeded(image, 60, 60);
                    smallImage.Save(smallImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    image = ResizeImage.ResizeImageIfNeeded(image, 780, 780);
                    image.Save(bigImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    GetUser.ProfilePhoto = GetUser.HtmlName + "_prof.jpg";
                    repository.Save();
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Profil.FotkaPost");
                    ModelState.AddModelError("", "Při ukládání fotky nastala chyba.");
                }
                finally
                {
                    if (image != null)
                        image.Dispose();
                    if (smallImage != null)
                        smallImage.Dispose();
                }
            }
            else
            {
                ModelState.AddModelError("", "Není vybrán žádný soubor.");
            }

            if (ModelState.IsValid)
            {
                CreateToastrMessage("Fotka byla úspěšně uložena."
                    + "Pokud se stál zobrazuje stará fotka, je nutno pročistit cache prohlížeče stiskem CTRL+F5");
                return RedirectToAction("Detail", "Profil");
            }

            var model = new ProfilPhotoViewModel();
            AddActionButtons(model);

            return View(model);
        }
        #endregion

        #region Settings
        [Authorize]
        public ActionResult Nastaveni()
        {
            var model = new ProfilViewModel()
            {
                BirhtYear = GetUser.Profiles.BirhtYear,
                City = GetUser.Profiles.City,
                Email = GetUser.Email,
                Hobbies = GetUser.Profiles.Hobbies,
                Link = GetUser.Profiles.Link,
                Motto = GetUser.Profiles.Motto,
                Name = GetUser.Profiles.Name,
                PhoneNumber = GetUser.PhoneNumber,
                Status = GetUser.Profiles.Status,
                Sex = GetUser.Profiles.Sex,
            
                Title = "Nastavení profilu",
                Icon = "icon-cogs",
            };

            AddActionButtons(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Nastaveni")]
        public ActionResult NastaveniPost(ProfilViewModel model)
        {
            CheckEmailAddress(model.Email);

            if (!ModelState.IsValid)
                return View(model);

            if (GetUser.Email != model.Email)
            {
                logger.LogInfo(
                    $"Změna emailové adresy z {GetUser.Email} na {model.Email} u uživatele {GetUser.UserName}",
                    "Profil.NastaveniPost");
            }

            GetUser.Profiles.BirhtYear = model.BirhtYear;
            GetUser.Profiles.City = model.City;
            GetUser.Email = model.Email;
            GetUser.Profiles.Hobbies = model.Hobbies;
            GetUser.Profiles.Link = model.Link;
            GetUser.Profiles.Motto = model.Motto;
            GetUser.Profiles.Name = model.Name;
            GetUser.PhoneNumber = model.PhoneNumber;
            GetUser.Profiles.Status = model.Status;
            GetUser.Profiles.Sex = model.Sex;
            repository.Save();

            CreateToastrMessage("Změny byly úspěšně uloženy.");

            return RedirectToAction("Detail", "Profil");
        }

        private void CheckEmailAddress(string email)
        {
            try
            {
                var message = new System.Net.Mail.MailMessage();
                message.To.Add(email);
            }
            catch 
            {
                ModelState.AddModelError("", "Neplatná emailová adresa");
            }
        }
        #endregion

        #region Transaction
        public ActionResult Transakce()
        {
            var userId = GetUserId;
            var model = new TransakceViewModel()
            {
                Title = "Historie transakcí",
                Icon = "icon-money",
                Data = repository
                    .Where<Payment>(p => p.Id_User == userId && p.Status == PaymentStatus.Paid)
                    .ToArray()
                    .Select(pe => new []
                    {
                        pe.Note,
                        pe.Amount.ToString(),
                        pe.CreateDate?.ToString(Strings.DateFormat),
                        pe.UpdateDate?.ToString(Strings.DateFormat),
                    })
                    .ToArray(),
            };

            return View(model);
        }
        #endregion

        #region VOP
        public ActionResult Vop()
        {
            var user = GetUser;
            var model = new VopViewModel
            {
                Gdpr = true,
                Vop = user.VopVersion == Settings.MinVopVersion
            };


            return View(new GenericBaseViewModel<VopViewModel>(model));
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Vop")]
        public ActionResult VopPost(VopViewModel model)
        {
            var user = repository.OneByHtml<AspNetUsers>(GetUserHtmlName);
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            if (model.Vop)
            {
                // souhlasil s nemá podmínky z minula -> uložit nový souhlas
                if (user.VopVersion < Settings.MinVopVersion)
                {
                    user.VopVersion = Settings.MinVopVersion;
                    repository.Save();

                    ApplicationUser appUser = this.userManager.FindById(GetUserId);
                    signInManager.SignIn(appUser, true, true);
                }
            }
            else
            {
                user.VopVersion = 0;
                repository.Save();

                var claimVop = claimsIdentity.FindFirst("VOP");
                if (claimVop != null)
                {
                    claimsIdentity.RemoveClaim(claimVop);
                }
            }

            var viewModel = new VopViewModel
            {
                Gdpr = true,
                Vop = user.VopVersion == Settings.MinVopVersion
            };


            return View(new GenericBaseViewModel<VopViewModel>(viewModel));
        }
        #endregion

        #region Emaily
        public ViewResult Emaily()
        {
            var user = GetUser;
            var model = new EmailSettingsViewModel
            {
                SendMayBeEventNotice = user.Profiles.SendMayBeEventNotice,
                SendMessagesFromAdminToMail = user.Profiles.SendMessagesFromAdminToMail,
                SendMessagesToMail = user.Profiles.SendMessagesToMail,
                SendNewAlbumsToMail = user.Profiles.SendNewAlbumsToMail,
                SendNewSummaryToMail = user.Profiles.SendNewSummaryToMail,
                Stay = user.Profiles.SendEventIsStayMail,
            };

            foreach (MainCategory mainCategory in Enum.GetValues(typeof(MainCategory)))
            {
                model.MainCategories.Add(new EmailCategory()
                {
                    Id = (int)mainCategory,
                    Name = mainCategory.GetDescription(),
                    MainCategory = mainCategory,
                    IsAssigned = user.MainCategories.Any(x => x.MainCategory == mainCategory),
                });
            }

            var allCategories = repository.All<EventCategory>();
            var userCategoryIds = user.EventCategories.Select(x => x.Id).ToArray();
            foreach (var category in allCategories)
            {
                model.Categories.Add(new EmailCategory()
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsAssigned = userCategoryIds.Contains(category.Id),
                    MainCategory = category.MainCategory
                });
            }

            AddActionButtons(model);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Emaily")]
        public ActionResult EmailyPost(EmailSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    CreateToastrMessage("Vaše nastavení bylo uloženo");
                    var profil = GetUser.Profiles;
                    profil.SendMayBeEventNotice = model.SendMayBeEventNotice;
                    profil.SendMessagesFromAdminToMail = model.SendMessagesFromAdminToMail;
                    profil.SendMessagesToMail = model.SendMessagesToMail;
                    profil.SendNewAlbumsToMail = model.SendNewAlbumsToMail;
                    profil.SendNewSummaryToMail = model.SendNewSummaryToMail;
                    profil.SendEventIsStayMail = model.Stay;

                    SaveMainCategory(GetUser, model.MainCategoryIds);
                    SaveEventCategory(GetUser, model.CategoryIds);
                    repository.Save();
                }
                catch(Exception ex)
                {
                    logger.LogException(ex, "Profil.EmailPost");
                    CreateToastrMessage("Chyba při ukládání vašeho nastavení");
                }
            }

            return RedirectToAction("Emaily", "Profil");
        }

        private void SaveEventCategory(AspNetUsers user, int[] categoryIds)
        {
            var userEventCategories = user.EventCategories.ToArray();
            // remove unchoosen category
            foreach (var category in userEventCategories)
            {
                if (!categoryIds.Contains(category.Id))
                {
                    user.EventCategories.Remove(category);
                }
            }

            // add choosen category
            foreach (var categoryId in categoryIds)
            {
                if (!userEventCategories.Any(x => x.Id == categoryId))
                {
                    user.EventCategories.Add(repository.One<EventCategory>(categoryId));
                }
            }
        }

        private void SaveMainCategory(AspNetUsers user, int[] mainCategoryIds)
        {
            var userCategories = user.MainCategories.ToArray();
            // remove unchoosen category
            foreach(var category in userCategories)
            {
                if (!mainCategoryIds.Contains((int)category.MainCategory))
                {
                    user.MainCategories.Remove(category);
                }
            }

            // add choosen category
            foreach(var categoryId in mainCategoryIds)
            {
                if (!userCategories.Any(x => (int)x.MainCategory == categoryId))
                {
                    user.MainCategories.Add(new AspNetUsersMainCategory { MainCategory = (MainCategory)categoryId });
                }
            }
        }
        #endregion

        private void AddActionButtons(BaseViewModel model)
        {
            model.Buttons.Add(CreateActionButton("Profil", "~/Profil/Detail", "icon-user"));
            model.Buttons.Add(CreateActionButton("Upravit profil", "~/Profil/Nastaveni", "icon-gear"));
            model.Buttons.Add(CreateActionButton("Emailová upozornění", "~/Profil/Emaily", "icon-warning-sign"));
            model.Buttons.Add(CreateActionButton("Nahrát profilovou fotku", "~/Profil/Fotka", "icon-camera"));
        }
    }
}