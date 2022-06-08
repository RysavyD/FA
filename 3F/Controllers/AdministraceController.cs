using System.Net;
using _3F.Model.Service;

namespace _3F.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;
    using _3F.Model;
    using _3F.Model.Accounting;
    using _3F.Model.Email;
    using _3F.Model.Email.Model;
    using _3F.Model.Extensions;
    using _3F.Model.Model;
    using _3F.Model.Repositories;
    using _3F.Model.Repositories.Interface;
    using _3F.Model.Utils;
    using _3F.Web.Definitions;
    using _3F.Web.Helpers;
    using _3F.Web.Models;
    using _3F.Web.Models.Administration;
    using _3F.Web.Utils;
    using Microsoft.AspNet.Identity;
    using Post = BusinessEntities.Post;

    [Authorize(Roles = "Administrator,Council,Supervisor")]
    public class AdministraceController : BaseController
    {
        private const string PostFileManagerPath = "~/App_Data/PostUpload";
        private readonly ApplicationUserManager userManager;
        private readonly ILogToEvent logToEvent;
        private readonly IAccounting accounting;
        private readonly IEmailSender emailSender;
        private readonly IHelpRepository helpRepository;
        private readonly IEventCategoryRepository eventCategoryRepository;
        private readonly IPostRepository postRepository;
        private readonly IFileUploadInfoRepository fileUploadInfoRepository;
        private readonly IKnowFromRepository knowFromRepository;
        private readonly IOrganizationMembersRepository organizationMembersRepository;
        private readonly IMessageService messageService;
        private readonly IRajceService rajceService;
        private readonly IEventUtils eventUtils;

        private readonly string[] roleForCouncil =
        {
            "Accountant", "EventCreator", "BadmintonAdmin", "BadmintonUser",
            "CertifiedOrganisator", "PrivateOrganisator", "Council", "Supervisor", "Chief"
        };

        public AdministraceController(
            ApplicationUserManager userManager, 
            ILogToEvent logToEvent,
            IAccounting accounting, 
            IEmailSender emailSender, 
            IHelpRepository helprepository,
            IEventCategoryRepository eventCategoryrepository, 
            IPostRepository postrepository, 
            IFileUploadInfoRepository fileUploadrepository,
            IKnowFromRepository knowFromrepository, 
            IOrganizationMembersRepository organizationMembersrepository,
            IMessageService messageService,
            IRajceService rajceService,
            IEventUtils eventUtils)
        {
            this.userManager = userManager;
            this.logToEvent = logToEvent;
            this.accounting = accounting;
            this.emailSender = emailSender;
            this.helpRepository = helprepository;
            this.eventCategoryRepository = eventCategoryrepository;
            this.postRepository = postrepository;
            this.fileUploadInfoRepository = fileUploadrepository;
            this.knowFromRepository = knowFromrepository;
            this.organizationMembersRepository = organizationMembersrepository;
            this.messageService = messageService;
            this.rajceService = rajceService;
            this.eventUtils = eventUtils;
            this.BackgroundColor = "dark-background";
            this.Icon = "icon-cogs";
        }

        public ActionResult Index()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileInfo fileInfo = new FileInfo(assembly.Location);
            string time = fileInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
            ViewBag.IsAdmin = User.IsInRole(Strings.Administrator);

            return this.View(new EmptyBaseViewModel() { Title = $"Administrace (verze {time})" });
        }

        #region Logy
        public ActionResult Logy(string id, string typ)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                string[] files = Directory.GetFiles(Server.MapPath("~/Logs"), "*.log*");
                Array.Sort(files, (u, v) => string.Compare(v, u, StringComparison.Ordinal));
                var logNameList = files.Select(f => new LogName(f));
                return this.View("LogList", logNameList);
            }
            else
            {
                string logName = id + ".log";
                if (!string.IsNullOrWhiteSpace(typ))
                {
                    logName = logName + typ;
                }
                
                if (System.IO.File.Exists(Server.MapPath("~/Logs") + "/" + logName))
                {
                    var lines = System.IO.File.ReadAllLines(Server.MapPath("~/Logs") + "/" + logName, System.Text.Encoding.UTF8);
                    if (string.IsNullOrWhiteSpace(typ))
                    {
                        List<LogUnit> items = new List<LogUnit>();
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Split(new[] { ";" }, StringSplitOptions.None).Length < 3)
                            {
                                items.Add(new LogUnit(lines[i] + lines[i + 1]));
                                i++;
                            }
                            else
                            {
                                items.Add(new LogUnit(lines[i]));
                            }
                        }

                        ViewBag.Title = logName;
                        return this.View("LogDetail", items);
                    }
                    else
                    {
                        return this.View("LogComplete", lines);
                    }
                }
                else
                {
                    return this.View("LogDetail", new List<LogUnit>());
                }
            }
        }
        #endregion

        #region Přehled uživatelů
        public ActionResult PrehledUzivatelu()
        {
            return this.View(new EmptyBaseViewModel("Přehled uživatelů"));
        }
        #endregion

        #region Přehled pravidelných akcí
        public ActionResult PrehledPravidelnychAkci()
        {
            var result = repository.All<PeriodicEvent>()
                .ToArray()
                .Select(pe => new[]
                {
                    pe.Id.ToString(),
                    pe.EventNameFormat,
                    pe.PeriodicEventType.ToString(),
                    this.GetParameterString(pe),
                });

            return this.View(result);
        }

        private string GetParameterString(PeriodicEvent periodicEvent)
        {
            if (periodicEvent.PeriodicEventType == PeriodicEventTypeEnum.DayInWeek)
            {
                return DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)periodicEvent.PeriodicParameter);
            }

            return periodicEvent.PeriodicParameter.ToString();
        }
        #endregion

        #region Nastavení práv uživatelů
        public ActionResult Uzivatele()
        {
            var model = repository.Where<AspNetUsers>(u => u.LoginType == LoginTypeEnum.Confirmed || u.LoginType == LoginTypeEnum.OldSystemConfirmed)
                .OrderBy(u => u.UserName)
                .Select(u => new UserWithRoles()
                {
                    htmlName = u.HtmlName,
                    name = u.UserName,
                    ProfilePhoto = u.ProfilePhoto,
                })
                .ToArray();

            return this.View(model);
        }

        public ActionResult Uzivatel(string id)
        {
            var user = repository.OneByHtml<AspNetUsers>(id);

            var model = new GenericBaseViewModel<UserWithRoles>(new UserWithRoles()
            {
                htmlName = user.HtmlName,
                name = user.UserName,
                ProfilePhoto = user.ProfilePhoto,
                Email = user.Email,
            })
            {
                Title = "Nastavení uživatele " + user.UserName
            };

            foreach (var roleName in Enum.GetNames(typeof(RolesEnum)))
            {
                model.Entity.Roles.Add(new RoleViewModel()
                    {
                        RoleName = roleName,
                        RoleDescription = this.GetRoleDescription(roleName),
                        IsInRole = user.AspNetRoles.Any(r => r.Name == roleName),
                        IsEditable = this.IsRoleEditable(roleName),
                    });
            }

            return this.View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Uzivatel")]
        public async Task<ActionResult> UzivatelPost(string id, string[] roles)
        {
            try
            {
                if (roles == null)
                {
                    roles = new string[] { };
                }

                int userId = repository.One<AspNetUsers>(u => u.HtmlName == id).Id;

                foreach (var roleName in Enum.GetNames(typeof(RolesEnum)))
                {
                    var userInRole = await this.userManager.IsInRoleAsync(userId, roleName);
                    if (roles.Contains(roleName) && !userInRole)
                    {
                        await this.userManager.AddToRoleAsync(userId, roleName);
                    }
                    else if (!roles.Contains(roleName) && userInRole)
                    {
                        await this.userManager.RemoveFromRoleAsync(userId, roleName);
                    }
                }

                this.CreateToastrMessage("Role byly úspěšně nastaveny.");
            }
            catch (Exception ex)
            {
                logger.LogException(ex, "Administrace.UzivatelPost");
                this.CreateToastrMessage("Při nastavování rolí došlo k chybě.");
            }

            return this.RedirectToAction("Uzivatele", "Administrace");
        }

        private string GetRoleDescription(string roleName)
        {
            return ((RolesEnum)Enum.Parse(typeof(RolesEnum), roleName)).GetDescription();
        }

        private bool IsRoleEditable(string roleName)
        {
            if (User.IsInRole(Strings.Administrator))
            {
                return true;
            }

            return this.roleForCouncil.Contains(roleName);
        }
        #endregion

        #region Smazat uživatele
        public ActionResult SmazatUzivatele()
        {
            return this.View("~/Views/Administrace/SmazatUzivatele.cshtml", string.Empty, string.Empty);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("SmazatUzivatele")]
        public ActionResult SmazatUzivatelePost(string userName)
        {
            var user = repository.One<AspNetUsers>(u => u.UserName == userName);
            if (user == null)
            {
                ModelState.AddModelError($"Uživatel \"{userName}\" nenalezen.");
            }

            if (!ModelState.IsValid)
            {
                return this.View("~/Views/Administrace/SmazatUzivatele.cshtml", string.Empty, userName);
            }

            int i = 1;
            while (repository.One<AspNetUsers>(u => u.UserName == $"Smazaný{i}") != null)
            {
                i++;
            }

            user.UserName = $"Smazaný{i}";
            user.HtmlName = $"Smazaný{i}";
            user.LoginType = LoginTypeEnum.Deleted;
            user.ProfilePhoto = "Unknown_prof.jpg";

            user.Profiles.BirhtYear = null;
            user.Profiles.City = string.Empty;
            user.Profiles.Hobbies = string.Empty;
            user.Profiles.Link = string.Empty;
            user.Profiles.Motto = string.Empty;
            user.Profiles.Name = string.Empty;
            user.Profiles.SendMayBeEventNotice = false;
            user.Profiles.SendMessagesFromAdminToMail = false;
            user.Profiles.SendMessagesToMail = false;
            user.Profiles.SendNewActionToMail = false;
            user.Profiles.Sex = SexEnum.Undefined;
            user.Profiles.Status = RelationshipStatus.Undefined;

            repository.Save();

            var events = repository.Where<Event>(ev => ev.StartDateTime > Info.CentralEuropeNow && ev.State == EventStateEnum.Active)
                .Where(l => l.EventParticipant
                    .Any(m => m.Id_User == user.Id &&
                        (m.EventLoginStatus == EventLoginEnum.Nahradnik ||
                        m.EventLoginStatus == EventLoginEnum.Prijdu ||
                        m.EventLoginStatus == EventLoginEnum.Rezervace)))
                .ToArray();

            foreach (var ev in events)
            {
                this.logToEvent.LogToEvent(ev, user, EventLoginEnum.Neprijdu, false);
            }

            repository.Save();

            var payments = repository.Where<Payment>(p => p.Id_User == user.Id && p.Status == PaymentStatus.Active);
            foreach (var payment in payments)
            {
                payment.Status = PaymentStatus.Cancelled;
                payment.UpdateDate = Info.CentralEuropeNow;
            }

            repository.Save();

            logger.LogInfo($"Smazán uživatel {userName}, jeho jméno je nyní Smazany{i}", "Administrace.SmazatUzivatele");
            this.CreateToastrMessage($"Uživatel \"{userName}\" byl smazán.");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Přehled rolí
        public ActionResult PrehledRoli()
        {
            var model = repository.All<AspNetRoles>()
                .OrderBy(r => r.Name)
                .AsEnumerable()
                .Select(r => new RoleWithUsers()
                {
                    Name = ((RolesEnum)Enum.Parse(typeof(RolesEnum), r.Name)).GetDescription(),
                    Users = r.AspNetUsers
                        .OrderBy(u => u.UserName)
                        .Select(u => new User()
                        {
                            htmlName = u.HtmlName,
                            name = u.UserName,
                            ProfilePhoto = u.ProfilePhoto,
                        })
                });

            return this.View(model);
        }
        #endregion

        #region Přehled plateb
        public ActionResult PrehledPlateb()
        {
            var result = repository.All<Payment>()
                .ToArray()
                .Select(pe => new[]
                {
                    pe.Id.ToString(),
                    pe.AspNetUsers.UserName,
                    pe.AspNetUsers.VariableSymbol.ToString(),
                    this.GetPaymentStatusString(pe),
                    pe.Amount.ToString(),
                    pe.CreateDate.HasValue ? pe.CreateDate.Value.ToString(Strings.DateFormat) : string.Empty,
                    pe.UpdateDate.HasValue ? pe.UpdateDate.Value.ToString(Strings.DateFormat) : string.Empty,
                    pe.Note,
                    pe.Id.ToString(),
                })
                .ToArray();

            var model = new EnumerableBaseViewModel<string[]>(result)
            {
                Title = "Přehled plateb",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        private string GetPaymentStatusString(Payment payment)
        {
            return payment.Status.GetDescription();
        }
        #endregion

        #region Nová platba
        public ActionResult NovaPlatba()
        {
            var model = new NewPaymentModel();

            return this.View(this.FillNewPaymentModel(model));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NovaPlatba(NewPaymentModel model)
        {
            if (model.Price == 0)
            {
                ModelState.AddModelError("Částka nesmí být nulová.");
            }

            if (model.Type == "jiny" && string.IsNullOrWhiteSpace(model.Reason))
            {
                ModelState.AddModelError("Musí být uveden důvod.");
            }

            if (!ModelState.IsValid)
            {
                return this.View(this.FillNewPaymentModel(model));
            }

            var eventEntity = repository.OneByHtml<Event>(model.HtmlName);
            string note = this.CreateNoteForPayment(model, eventEntity);
                
            var payment = new Payment()
            {
                AspNetUsers = repository.OneByHtml<AspNetUsers>(model.UserName),
                Amount = model.Price,
                CreateDate = Info.CentralEuropeNow,
                Event = eventEntity,
                Status = PaymentStatus.Active,
                Note = note,
            };

            repository.Add(payment);
            this.CreateToastrMessage("Platba byla úspěšně přidána");

            return this.RedirectToAction(string.Empty, "Administrace");
        }

        private string CreateNoteForPayment(NewPaymentModel model, Event eventEntity)
        {
            switch (model.Type)
            {
                case "ucast":
                    return $"Účast na akci {eventEntity.Name}";
                case "odhlaseni":
                    return $"Odhlášení z akce {eventEntity.Name} v termínu";
                case "refundace":
                    return $"Refundace za akci {eventEntity.Name}, refundováno {model.Price}";
                case "externi":
                    return $"Platba za externistu na akci {eventEntity.Name}";
                default:
                    return model.Reason;
            }
        }

        private NewPaymentModel FillNewPaymentModel(NewPaymentModel model)
        {
            model.Events = repository.Where<Event>(ev => ev.EventType == EventTypeEnum.PlacenaSdruzenim)
                            .OrderByDescending(ev => ev.AccountSymbol)
                            .ToArray()
                            .Select(ev => new SelectListItem()
                            {
                                Text = $"{ev.AccountSymbol} - {ev.Name}" + ((ev.State == EventStateEnum.Deleted) ? " (smazáno)" : string.Empty),
                                Value = ev.HtmlName,
                            });

            model.Users = repository.Where<AspNetUsers>(u => u.VariableSymbol != null && u.LoginType != LoginTypeEnum.Deleted)
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem()
                {
                    Text = u.UserName,
                    Value = u.HtmlName,
                });

            model.Types = new List<SelectListItem>()
            {
                new SelectListItem() { Text = "Účast na akci", Value = "ucast" },
                new SelectListItem() { Text = "Odhlášení z akce", Value = "odhlaseni" },
                new SelectListItem() { Text = "Refundace", Value = "refundace" },
                new SelectListItem() { Text = "Platba za externího účastníka", Value = "externi" },
                new SelectListItem() { Text = "Jiný důvod", Value = "jiny" },
            };

            model.Title = "Přidání nové platby";

            return model;
        }
        #endregion

        #region Zaplatit platbu
        public ActionResult ZaplatitPlatbu()
        {
            return this.View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ZaplatitPlatbu(int paymentId)
        {
            var payment = repository.One<Payment>(paymentId);

            logger.LogDebug(
                $"Ruční požadavek na zaplacení platby, ID:{payment.Id}, uživatel:{payment.AspNetUsers.UserName}, "
                    + $"akce:{payment.Event.Name}, částka:{payment.Amount}, poznámka:{payment.Note}, zaplaceno:{payment.Status.GetDescription()}",
                "Administrace.ZaplatitPlatbu");

            if (payment.Status != PaymentStatus.Active)
            {
                this.CreateToastrMessage($"Platba s Id: {paymentId} nebyla zaplacena. Je ve stavu: {payment.Status.GetDescription()}.");
                return this.RedirectToAction(string.Empty, "Administrace");
            }

            this.logToEvent.PaidPayment(payment);
            this.CreateToastrMessage($"Aktuální stav platby ID:{paymentId} je {payment.Status.GetDescription()}");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Zkontrolovat a přidělit VS
        public ActionResult ZkontrolovatVS()
        {
            return this.View(new UserWithSymbol());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> ZkontrolovatVS(UserWithSymbol model, string submit)
        {
            if (submit == "Zkontrolovat VS")
            {
                var user = repository.One<AspNetUsers>(u => u.UserName == model.UserName);
                if (user != null)
                {
                    if (user.VariableSymbol == null)
                    {
                        var data = await this.accounting.GetData(user.Email);
                        if (data.CommunicationOk)
                        {
                            if (data.VS != 0)
                            {
                                user.VariableSymbol = data.VS;
                                repository.Save();
                                model.Message = $"Uživatel {model.UserName} nalezen v pokladně, přidělen VS: {data.VS}";
                            }
                            else
                            {
                                model.Message = $"Uživatel {model.UserName} nenalezen v platebním systému";
                            }
                        }
                        else
                        {
                            model.Message = "Chyba při komunikaci s platebním systémem";
                        }
                    }
                    else
                    {
                        model.Message = $"Uživatel {model.UserName} již má VS: {user.VariableSymbol}";
                    }
                }
                else
                {
                    model.Message = $"Uživatel {model.UserName} nebyl nalezen v systému";
                }
            }
            else if (submit == "Přidělit VS")
            {
                var user = repository.One<AspNetUsers>(u => u.UserName == model.UserName);
                if (user != null)
                {
                    if (user.VariableSymbol == null)
                    {
                        var data = await this.accounting.GetNewUserSymbol(user.Email, user.UserName);
                        if (string.IsNullOrWhiteSpace(data.Problem))
                        {
                            user.VariableSymbol = data.Symbol;
                            repository.Save();
                            model.Message = $"Uživateli {model.UserName} byl přidělen VS: {data.Symbol}";
                        }
                        else
                        {
                            model.Message = "Chyba při komunikaci s platebním systémem";
                        }
                    }
                    else
                    {
                        model.Message = $"Uživatel {model.UserName} již má VS: {user.VariableSymbol}";
                    }
                }
                else
                {
                    model.Message = $"Uživatel {model.UserName} nebyl nalezen v systému";
                }
            }

            return this.View(model);
        }
        #endregion

        #region Nezaplacené platby
        public ActionResult Nezaplaceno()
        {
            var model = new EnumerableBaseViewModel<PaymentViewModel>
            {
                Title = "Nezaplacené platby",
                Entities = repository.Where<Payment>(p => p.Status == PaymentStatus.Active)
                    .ToArray()
                    .Select(p => new PaymentViewModel()
                    {
                        Id = p.Id,
                        UserName = p.AspNetUsers.UserName,
                        Amount = p.Amount,
                        CreateDate = p.CreateDate.Value.ToString("dd.MM.yyyy hh:mm:ss"),
                        EventName = p.Event.Name,
                        Note = p.Note,
                        EventHtml = p.Event.HtmlName,
                    })
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        public ActionResult Zaplatit(int id)
        {
            var payment = repository.One<Payment>(id);
            if (payment == null || payment.Status != PaymentStatus.Active)
            {
                this.CreateToastrMessage("Nenalezena nezaplacená platba.");
            }

            logger.LogDebug($"Platba {payment.Id} byla ručně zaplacena administrátorem", "Administrace.Zaplatit");
            this.logToEvent.PaidPayment(payment);

            this.CreateToastrMessage($"Platba byla vykonána. Stav platby je {payment.Status.GetDescription()}");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Rezervace
        public ActionResult Rezervace()
        {
            var model = new EnumerableBaseViewModel<Reservation>()
            {
                Title = "Přehled rezervací",
                Entities = repository.Where<EventParticipant>(ev =>
                        ev.EventLoginStatus == EventLoginEnum.Rezervace ||
                        ev.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace)
                    .Where(ev => ev.Event.StartDateTime >= Info.CentralEuropeNow)
                    .OrderByDescending(ev => ev.Time)
                    .Select(p => new Reservation()
                    {
                        CreateDate = p.Time,
                        EventName = p.Event.Name,
                        EventHtmlName = p.Event.HtmlName,
                        Id = p.Id,
                        Status = p.EventLoginStatus.ToString(),
                        UserName = p.AspNetUsers.UserName,
                        UserHtmlName = p.AspNetUsers.HtmlName,
                    })
            };

            return this.View(model);
        }

        public ActionResult SolveReservation(int id)
        {
            var participant = repository.One<EventParticipant>(id);
            string result = this.logToEvent.CheckReservation(participant);
            this.CreateToastrMessage(result);

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Email od admina
        public ActionResult AdminEmail()
        {
            return this.View("~/Views/Administrace/AdminEmail.cshtml", new AdminEmail() { Title = "Email od administrátora" });
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("AdminEmail"), ValidateInput(false)]
        public ActionResult AdminEmailPost(AdminEmail model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var emails = repository.Where<AspNetUsers>(u => u.Profiles.SendMessagesFromAdminToMail)
                .Where(u => u.LoginType == LoginTypeEnum.Confirmed || u.LoginType == LoginTypeEnum.OldSystemConfirmed)
                .Select(u => u.Email)
                .ToArray();

            var email = new BaseEmailModel() { Body = model.Body, Subject = model.Subject };
            this.emailSender.SendEmail(EmailType.InfoFromAdmin, email, emails);

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Novinka
        public ActionResult News()
        {
            var model = new NewsViewModel();
            string path = Path.Combine(Values.Instance.AppDataPath, "news.txt");
            if (System.IO.File.Exists(path))
            {
                model.News = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
            }

            return this.View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("News"), ValidateInput(false)]
        public ActionResult NewsPost(NewsViewModel model)
        {
            string path = Path.Combine(Values.Instance.AppDataPath, "news.txt");
            System.IO.File.WriteAllText(path, model.News, System.Text.Encoding.UTF8);

            this.CreateToastrMessage("Novinky byla úspěšně uložena");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Upravit platbu
        public ActionResult ChangePayment(int id)
        {
            var payment = repository.One<Payment>(id);

            var model = new ChangePaymentViewModel()
            {
                Amount = payment.Amount,
                CreateDate = payment.CreateDate.Value,
                EventName = payment.Event.Name,
                Note = payment.Note,
                Status = payment.Status.ToString(),
                UserName = payment.AspNetUsers.UserName,
                UpdateDate = payment.UpdateDate,
                Title = "Změna platby",
            };

            var list = new List<SelectListItem>();
            foreach (PaymentStatus status in Enum.GetValues(typeof(PaymentStatus)))
            {
                list.Add(new SelectListItem()
                {
                    Text = status.GetDescription(),
                    Value = status.ToString(),
                    Selected = model.Status == status.GetDescription(),
                });
            }

            model.StatusList = list;

            return this.View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("ChangePayment")]
        public ActionResult ChangePaymentPost(int id, ChangePaymentViewModel model)
        {
            var payment = repository.One<Payment>(id);

            payment.Note = model.Note;
            payment.Status = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), model.Status);
            payment.Amount = model.Amount;
            payment.UpdateDate = Info.CentralEuropeNow;
            repository.Save();

            logger.LogInfo($"Změna platby {id} na částku {model.Amount} a stav {model.Status}", "Administrace.ChangePayment");

            this.CreateToastrMessage("Platba byla úspěšně změněna");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Upravit přihlášení
        public ActionResult ChangeEventParticipant(int id)
        {
            var participant = repository.One<EventParticipant>(id);

            var model = new ChangeEventParticipantModel()
            {
                Id = participant.Id,
                OldStatus = participant.EventLoginStatus.GetDescription(),
                EventName = participant.Event.Name,
                UserName = participant.AspNetUsers.UserName,
                Title = "Změna stavu přihlášení",
            };

            var list = new List<SelectListItem>();
            foreach (EventLoginEnum status in Enum.GetValues(typeof(EventLoginEnum)))
            {
                list.Add(new SelectListItem()
                {
                    Text = status.GetDescription(),
                    Value = status.ToString(),
                    Selected = model.OldStatus == status.GetDescription(),
                });
            }

            model.StatusList = list;

            return this.View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("ChangeEventParticipant")]
        public ActionResult ChangeEventParticipantPost(int id, ChangeEventParticipantModel model)
        {
            var participant = repository.One<EventParticipant>(id);
            var oldStatus = participant.EventLoginStatus; 
            participant.EventLoginStatus = (EventLoginEnum)Enum.Parse(typeof(EventLoginEnum), model.Status);

            repository.Add(new EventParticipantHistory()
            {
                AspNetUsers = participant.AspNetUsers,
                Event = participant.Event,
                IsExternal = participant.IsExternal,
                OldEventLoginStatus = oldStatus,
                NewEventLoginStatus = participant.EventLoginStatus,
                Time = Info.CentralEuropeNow,
                Id_Participant = participant.Id,
            });

            repository.Save();

            logger.LogInfo(
                $"Administrátor {User.Identity.Name} změnil stav přihlášení z {oldStatus.GetDescription()} "
                    + $"na {model.Status} u přihlášení s Id {model.Id}",
                "Administrace.ChangeEventParticipantPost");

            this.CreateToastrMessage("Přihlášení bylo úspěšně změněno");

            return this.RedirectToAction(string.Empty, "Administrace");
        }
        #endregion

        #region Přehled nároků na organizování
        public ActionResult NewOrganisators()
        {
            var lastYearDate = Info.CentralEuropeNow.AddYears(-1);
            var model = new EnumerableBaseViewModel<NewOrganisator>()
            {
                Entities = repository.Where<Event>(ev => ev.State == EventStateEnum.Active && ev.StartDateTime >= lastYearDate)
                    .SelectMany(ev => ev.EventParticipant)
                    .Where(ep => !ep.IsExternal)
                    .Where(ep => ep.EventLoginStatus == EventLoginEnum.Prijdu)
                    .GroupBy(ep => ep.AspNetUsers)
                    .ToArray()
                    .Select(g => new NewOrganisator()
                    {
                        User = new User(g.Key),
                        EventCount = g.Count(),
                        IsInRole = g.Key.AspNetRoles.Any(r => r.Name == Strings.EventCreator)
                    })
                    .OrderByDescending(no => no.EventCount)
                    .ToArray(),
                Title = "Přehled účastí na akcích",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }
        #endregion

        #region Update fotoalba
        public ActionResult UpdatePhotoAlbum()
        {
            return this.View(new EmptyBaseViewModel("Updatování fotoalba"));
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName("UpdatePhotoAlbum")]
        public ActionResult UpdatePhotoAlbumPost(int albumId)
        {
            try
            {
                var album = repository.One<PhotoAlbum>(albumId);
                var albumData = rajceService.GetAlbum(album.AlbumLink);
                album.PhotoCount = albumData.Photos.Length;
                album.CoverPhotoLink = albumData.CoverPhoto.Thumbnail;

                repository.Save();

                this.CreateToastrMessage("Album bylo úspěšně updatováno.");

                return this.RedirectToAction(string.Empty, "Administrace");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Message);
                return this.View(new EmptyBaseViewModel("Updatování fotoalba"));
            }
        }
        #endregion

        #region Přidání fotoalba za někoho
        public ActionResult AddPhotoAlbum()
        {
            var model = new AddPhotoAlbum()
            {
                Events = repository.Where<Event>(ev => ev.State == EventStateEnum.Active)
                    .OrderByDescending(ev => ev.StartDateTime)
                    .ToArray()
                    .Select(ev => new SelectListItem()
                    {
                        Text = $"{ev.Name} ({ev.StartDateTime.ToShortDateString()})",
                        Value = ev.HtmlName,
                    }),

                Users = repository.Where<AspNetUsers>(u => u.LoginType != LoginTypeEnum.Deleted)
                    .OrderBy(u => u.UserName)
                    .ToArray()
                    .Select(u => new SelectListItem()
                    {
                        Text = u.UserName,
                        Value = u.HtmlName,
                    }),
            };

            var viewModel = new GenericBaseViewModel<AddPhotoAlbum>(model)
            {
                Title = "Přidání fotoalba za jiného uživatele",
            };

            viewModel.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(viewModel);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("AddPhotoAlbum")]
        public ActionResult AddPhotoAlbumPost(AddPhotoAlbum model)
        {
            var eventEntity = repository.OneByHtml<Event>(model.HtmlName);
            var user = repository.OneByHtml<AspNetUsers>(model.UserName);
            if (!string.IsNullOrWhiteSpace(model.Link))
            {
                try
                {
                    var album = rajceService.GetAlbum(model.Link);

                    var albumEntity = new PhotoAlbum()
                    {
                        AlbumLink = model.Link,
                        AspNetUsers = user,
                        CoverPhotoLink = album.CoverPhoto.Thumbnail,
                        Discussion = new Discussion()
                        {
                            IsAlone = false,
                            Name = user.UserName + "-" + eventEntity.Name,
                            HtmlName = user.HtmlName + "-" + eventEntity.HtmlName,
                        },
                        Event = eventEntity,
                        PhotoCount = album.Photos.Length
                    };

                    repository.Add(albumEntity);

                    logger.LogInfo($"Uživatel {GetUser.UserName} vložil album k akci {eventEntity.Name} za uživatele {user.UserName}", "Administrace.AddPhotoAlbumPost");

                    this.CreateToastrMessage("Album bylo úspěšně vloženo");
                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "Administrace.AddPhotoAlbumPost");
                    ModelState.AddModelError("Nastala neočekávaná chyba");
                }
            }
            else
            {
                ModelState.AddModelError("Musí být vložen odkaz");
                model.Events = repository.Where<Event>(ev => ev.State == EventStateEnum.Active)
                    .OrderByDescending(ev => ev.StartDateTime)
                    .ToArray()
                    .Select(ev => new SelectListItem()
                    {
                        Text = $"{ev.Name} ({ev.StartDateTime.ToShortDateString()})",
                        Value = ev.HtmlName,
                    });

                model.Users = repository.Where<AspNetUsers>(u => u.LoginType != LoginTypeEnum.Deleted)
                   .OrderBy(u => u.UserName)
                   .ToArray()
                   .Select(u => new SelectListItem()
                   {
                       Text = u.UserName,
                       Value = u.HtmlName,
                   });

                var viewModel = new GenericBaseViewModel<AddPhotoAlbum>(model)
                {
                    Title = "Přidání fotoalba za jiného uživatele",
                };

                viewModel.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

                return this.View(viewModel);
            }

            return this.RedirectToAction("Index");
        }
        #endregion

        #region Upload
        public ActionResult Upload()
        {
            string[] files = Directory.GetFiles(Server.MapPath("~/App_Data/Upload"))
                .Select(f => Path.GetFileName(f))
                .ToArray();
            Array.Sort(files);
            return this.View("Upload", files);
        }

        public ActionResult Download(string id)
        {
            string filename = Directory.GetFiles(Server.MapPath("~/App_Data/Upload"))
                .First(f => Path.GetFileNameWithoutExtension(f).Replace(".", "-") == id.Replace(".", "-"));

            string path = Path.Combine(Server.MapPath("~/App_Data/Upload"), filename);

            var contentType = MimeMapping.GetMimeMapping(filename);

            return this.File(path, contentType);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Upload")]
        public ActionResult UploadPost(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath("~/App_Data/Upload"), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                }
                catch (Exception ex)
                {
                    this.CreateToastrMessage(ex.Message);
                }
            }

            return this.RedirectToAction("Upload");
        }
        #endregion

        #region Kategorie
        public ActionResult Kategorie()
        {
            var model = new GenericBaseViewModel<IEnumerable<BusinessEntities.EventCategory>>(this.eventCategoryRepository.GetAll())
            {
                Title = "Kategorie akcí",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        public ActionResult KategorieEdit(int? id)
        {
            var entity = id.HasValue ? this.eventCategoryRepository.GetById(id.Value) : new BusinessEntities.EventCategory();
            return this.PartialView(entity);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), ActionName("KategorieEdit")]
        public ActionResult KategorieEditPost(BusinessEntities.EventCategory entity)
        {
            if (entity.Id > 0)
            {
                this.eventCategoryRepository.Update(entity);
            }
            else
            {
                this.eventCategoryRepository.Add(entity);
            }

            return this.RedirectToAction("Kategorie");
        }

        public PartialViewResult KategorieDelete(int id)
        {
            var entity = this.eventCategoryRepository.GetById(id);
            return this.PartialView(entity);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("KategorieDelete")]
        public RedirectToRouteResult KategorieDeletePost(int id, CategoryEdit model)
        {
            this.eventCategoryRepository.Delete(id);
            this.CreateToastrMessage("Kategorie byla smazána");

            return this.RedirectToAction("Kategorie");
        }
        #endregion

        #region Přehled finančního stavu akcí
        public ActionResult EventBalanceOverView()
        {
            var list = repository
                .Where<Event>(ev => ev.EventType == EventTypeEnum.PlacenaSdruzenim)
                .Where(ev => ev.State != EventStateEnum.Deleted)
                .OrderByDescending(ev => ev.StartDateTime)
                .ToArray()
                .Select(ev => new EventBalance()
                {
                    Name = ev.Name,
                    HtmlName = ev.HtmlName,
                    Costs = ev.Costs,
                    CostsDescription = ev.CostsDescription,
                    Capacity = ev.Capacity,
                    Price = ev.Price,
                    Organisators = string.Join(", ", ev.EventOrganisator.Select(eo => eo.AspNetUsers.UserName)),
                    Paid = ev.Payment.Where(pay => pay.Status == PaymentStatus.Paid).Sum(pay => pay.Amount) * (-1),
                    WillBePaid = ev.Payment.Where(pay => pay.Status == PaymentStatus.Active).Sum(pay => pay.Amount) * (-1),
                    StartDate = ev.StartDateTime.ToOnlyDateTimeString(),
                });

            return this.View(new EnumerableBaseViewModel<EventBalance>(list));
        }

        public ActionResult EventBalanceExtendedOverView()
        {
            var model = new EmptyBaseViewModel("Finanční bilance akcí");
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return View(model);
        }

        public JsonResult EventBalanceExtendedOverviewData(int id)
        {
            var model = repository
                .Where<Event>(ev => ev.EventType == EventTypeEnum.PlacenaSdruzenim)
                .Where(ev => ev.State != EventStateEnum.Deleted)
                .Where(ev => ev.StartDateTime.Year == id)
                .OrderByDescending(ev => ev.StartDateTime)
                .ToArray()
                .Select(ev => new[]
                {
                    ev.Id.ToString(),
                    ev.Name,
                    string.Join(", ", ev.EventOrganisator.Select(eo => eo.AspNetUsers.UserName)),
                    ev.StartDateTime.ToString(Strings.DateFormat),
                    ev.EventParticipant.Count(x=>x.EventLoginStatus == EventLoginEnum.Prijdu).ToString(),
                    ev.Capacity.ToString(),
                    ev.Price.ToString(),
                    (ev.Payment.Where(pay => pay.Status == PaymentStatus.Paid).Sum(pay => pay.Amount) * (-1)).ToString(), // 8 prijmy
                    ev.Costs.ToString(), // 9 vydaje
                    "0", // 10 prijmy - vydaje
                    "0", // 11 vydaje kasa 
                    "false", // 12 uzavreno
                    (ev.Payment.Where(pay => pay.Status == PaymentStatus.Active).Sum(pay => pay.Amount) * (-1)).ToString(), // 12 vyhledova bilance
                    ev.CostsDescription, // 14 popis nakladu
                    ev.HtmlName,
                    ev.AccountSymbol.ToString() // 16 cislo pro ucetnictvi
                })
                .ToArray();

            foreach (var entity in model)
            {
                entity[9] = (Convert.ToDecimal(entity[7]) - Convert.ToDecimal(entity[8])).ToString();
            }

            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Help
        public ActionResult Help()
        {
            var model = new EnumerableBaseViewModel<BusinessEntities.Help>(this.helpRepository.GetAll())
            {
                Title = "Nápověda",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        public ActionResult HelpEdit(int? id)
        {
            var helpEntity = id.HasValue ? this.helpRepository.GetById(id.Value) : new BusinessEntities.Help();
            return this.PartialView(helpEntity);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), ActionName("HelpEdit")]
        public ActionResult HelpEditPost(BusinessEntities.Help entity)
        {
            if (entity.Id > 0)
            {
                this.helpRepository.Update(entity);
            }
            else
            {
                this.helpRepository.Add(entity);
            }

            return this.RedirectToAction("Help");
        }
        #endregion

        #region Post
        public ActionResult Post()
        {
            var model = new EnumerableBaseViewModel<PostViewModel>(this.postRepository.GetAll().Select(p => new PostViewModel(p)))
            {
                Title = "Články",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        public ActionResult PostShow(int id)
        {
            var entity = this.postRepository.GetById(id);
            if (this.UserHasViewAccess(entity))
            {
                return this.PartialView(new PostViewModel(entity));
            }

            return this.PartialView("PostNoRights");
        }

        public ActionResult PostEdit(int? id)
        {
            var entity = id.HasValue ? this.postRepository.GetById(id.Value) : new BusinessEntities.Post();

            if (this.UserHasEditAccess(entity))
            {
                return this.PartialView(new PostViewModel(entity));
            }

            return this.PartialView("PostNoRights");
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("PostEdit")]
        public ActionResult PostEditPost(PostViewModel viewModel)
        {
            var entity = viewModel.ToBusinessEntity();
            entity.HtmlName = entity.Name.StringToHtmlLink();
            if (entity.Id > 0)
            {
                this.postRepository.Update(entity);
            }
            else
            {
                this.postRepository.Add(entity);
            }

            return this.RedirectToAction("Post");
        }

        public ActionResult PostAccess(int id)
        {
            var entity = this.postRepository.GetById(id);

            if (!this.UserHasEditAccess(entity))
            {
                return this.PartialView("PostNoRights");
            }

            var model = this.CreatePostAccessViewModel(entity);

            return this.PartialView(model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("PostAccess")]
        public ActionResult PostAccessPost(PostAccessViewModel viewModel)
        {
            var entity = this.postRepository.GetById(viewModel.Id);
            entity.EditPermissions = viewModel.SelectedEditRights == null
                ? string.Empty
                : string.Join(",", viewModel.SelectedEditRights);
            entity.ViewPermissions = viewModel.SelectedViewRights == null
                ? null
                : string.Join(",", viewModel.SelectedViewRights);

            this.postRepository.Update(entity);

            return this.RedirectToAction("Post");
        }

        private bool UserHasEditAccess(Post entity)
        {
            if (entity == null)
            {
                return false;
            }

            return this.UserHasAccessToPost(entity.EditPermissions);
        }

        private bool UserHasViewAccess(Post entity)
        {
            if (entity == null)
            {
                return false;
            }

            return this.UserHasAccessToPost(entity.ViewPermissions);
        }

        private bool UserHasAccessToPost(string permission)
        {
            if (string.IsNullOrEmpty(permission))
            {
                return true;
            }

            if (!User.Identity.IsAuthenticated)
            {
                return false;
            }

            var permissions = permission.Split(',');

            // hvězdička -> přístupné pro všechny přihlášené uživatele
            if (permissions.Contains("*"))
            {
                return true;
            }

            // procento -> přístupné pouze členům
            if (permissions.Contains("%"))
            {
                if (this.organizationMembersRepository.IsMember(this.GetUserId))
                {
                    return true;
                }
            }

            // není hvězdička -> zkontrolovat role a jména
            var roles = this.userManager.GetRoles(GetUserId);

            return permissions.Intersect(roles).Any() || permissions.Contains(User.Identity.Name);
        }

        private PostAccessViewModel CreatePostAccessViewModel(Post entity)
        {
            var model = new PostAccessViewModel
            {
                AvailableEditRights = this.GetListItems(entity.EditPermissions),
                AvailableViewRights = this.GetListItems(entity.ViewPermissions),
                Name = entity.Name,
                Id = entity.Id,
            };

            return model;
        }

        private IList<SelectListItem> GetListItems(string permissions)
        {
            var list = Enum.GetNames(typeof(RolesEnum))
                .Select(r => new SelectListItem()
                {
                    Text = ((RolesEnum)Enum.Parse(typeof(RolesEnum), r)).GetDescription(),
                    Value = r,
                    Selected = permissions != null && permissions.Contains(r),
                })
                .ToList();

            list.Add(new SelectListItem()
            {
                Text = "Přihlášený uživatel",
                Value = "*",
                Selected = permissions != null && permissions.Contains("*"),
            });

            list.Add(new SelectListItem()
            {
                Text = "Člen sdružení",
                Value = "%",
                Selected = permissions != null && permissions.Contains("%"),
            });

            return list;
        }
        #endregion

        #region Post upload files
        public ActionResult PostFileManager()
        {
            var files = this.fileUploadInfoRepository.GetFiles()
                .OrderBy(f => f.Name)
                .Select(f => new FileUploadInfoViewModel(f))
                .ToArray();

            foreach (var file in files)
            {
                file.Length = (new FileInfo(Server.MapPath(PostFileManagerPath + file.Path + file.Name)).Length / 1024).ToString();
            }

            var model = new EnumerableBaseViewModel<FileUploadInfoViewModel>(files)
            {
                Title = "Správa souborů"
            };
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");
            
            return this.View(model);
        }

        public ActionResult PostFileManagerUpload()
        {
            return this.PartialView();
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("PostFileManagerUpload")]
        public ActionResult PostFileManagerUploadPost(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                try
                {
                    string path = Path.Combine(Server.MapPath(PostFileManagerPath), Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                    var description = Request["Description"];
                    this.fileUploadInfoRepository.Add(new BusinessEntities.FileUploadInfo()
                    {
                        CreationDate = Info.CentralEuropeNow,
                        Name = file.FileName,
                        Path = "/",
                        Description = description,
                    });
                }
                catch (Exception ex)
                {
                    this.CreateToastrMessage(ex.Message);
                }
            }

            return this.RedirectToAction("PostFileManager");
        }

        public ActionResult PostFileManagerDelete(int id)
        {
            var model = this.fileUploadInfoRepository.GetById(id).Name;
            return this.PartialView(model: model);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("PostFileManagerDelete")]
        public ActionResult PostFileManagerDeletePost(int id)
        {
            try
            {
                var fileInfo = this.fileUploadInfoRepository.GetById(id);

                string path = Path.Combine(Server.MapPath(PostFileManagerPath), fileInfo.Name);
                System.IO.File.Delete(path);
                this.fileUploadInfoRepository.Remove(id);
            }
            catch (Exception ex)
            {
                this.CreateToastrMessage(ex.Message);
            }

            return this.RedirectToAction("PostFileManager");
        }
        #endregion

        #region Přehled akcí
        [Authorize(Roles = Strings.Administrator)]
        public ActionResult PrehledAkci()
        {
            var model = new EmptyBaseViewModel("Přehled akcí");
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }

        [Authorize(Roles = Strings.Administrator)]
        public ActionResult PrehledAkciEdit(string id)
        {
            var entity = repository.OneByHtml<Event>(id);
            var model = new EditEventViewModel()
            {
                Id = entity.Id,
                EventType = entity.EventType,
                Name = entity.Name,
                State = entity.State,
            };

            return this.PartialView(model);
        }

        [Authorize(Roles = Strings.Administrator)]
        [HttpPost, ActionName("PrehledAkciEdit"), ValidateAntiForgeryToken]
        public ActionResult PrehledAkciEditPost(EditEventViewModel model, string id)
        {
            var entity = repository.OneByHtml<Event>(id);
            logger.LogInfo(
                $"Administrátor {User.Identity.Name} změnil akci s Html={id}, stav {entity.State}=>{model.State} "
                    + $"a typ {entity.EventType}=>{model.EventType}",
                "Administrace.PrehledAkciEditPost");

            entity.State = model.State;
            entity.EventType = model.EventType;
            repository.Save();            

            return this.RedirectToAction("PrehledAkci");
        }
        #endregion

        #region Registrace - odkud o nás více
        public ActionResult KnowFrom()
        {
            var model = new EnumerableBaseViewModel<BusinessEntities.Account.KnowFrom>(this.knowFromRepository.GetAll())
            {
                Title = "Odkud jste se o nás dozvěděl",
            };

            AddBackButton(model);

            return this.View(model);
        }

        public ActionResult KnowFromEdit(int? id)
        {
            var entity = id.HasValue ? this.knowFromRepository.GetById(id.Value) : new BusinessEntities.Account.KnowFrom();
            return this.PartialView(entity);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), ActionName("KnowFromEdit")]
        public ActionResult KnowFromEditPost(BusinessEntities.Account.KnowFrom entity)
        {
            if (entity.Id > 0)
            {
                this.knowFromRepository.Update(entity);
            }
            else
            {
                this.knowFromRepository.Add(entity);
            }

            return this.RedirectToAction("KnowFrom");
        }

        public PartialViewResult KnowFromDelete(int id)
        {
            var entity = this.knowFromRepository.GetById(id);
            return this.PartialView(entity);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("KnowFromDelete")]
        public RedirectToRouteResult KnowFromDeletePost(int id, BusinessEntities.Account.KnowFrom model)
        {
            this.knowFromRepository.Delete(id);
            this.CreateToastrMessage("Položka byla smazána");

            return this.RedirectToAction("KnowFrom");
        }
        #endregion

        #region Čekající platby
        public ActionResult PlatbyCekajici()
        {
            var result = repository.All<Payment>()
                .Where(p => p.Status == PaymentStatus.Waiting)
                .ToArray()
                .Select(pe => new[]
                {
                                pe.Id.ToString(),
                                pe.AspNetUsers.UserName,
                                pe.AspNetUsers.VariableSymbol.ToString(),
                                this.GetPaymentStatusString(pe),
                                pe.Amount.ToString(),
                                pe.CreateDate.HasValue ? pe.CreateDate.Value.ToString(Strings.DateFormat) : string.Empty,
                                pe.UpdateDate.HasValue ? pe.UpdateDate.Value.ToString(Strings.DateFormat) : string.Empty,
                                pe.Note,
                                pe.Id.ToString(),
                })
                .ToArray();

            var model = new EnumerableBaseViewModel<string[]>(result)
            {
                Title = "Čekající platby",
            };

            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return this.View(model);
        }
        #endregion

        #region Převést na placenou akci
        [Authorize(Roles = Strings.Administrator)]
        public ActionResult ChangeToPaidEvent()
        {
            var events = repository.Where<Event>(e =>
                    e.StartDateTime > Info.CentralEuropeNow && e.EventType == EventTypeEnum.Bezna &&
                    e.State == EventStateEnum.Active)
                .ToArray()
                .Select(e => new Tuple<string, string, int>(e.Name, e.HtmlName, e.Id));

            var viewModel = new EnumerableBaseViewModel<Tuple<string, string, int>>(events)
            {
                Title = "Převést na placenou akci"
            };

            return View(viewModel);
        }

        [Authorize(Roles = Strings.Administrator)]
        public ActionResult ChangeToPaidEventEdit(int id)
        {
            var entity = repository.One<Event>(id);
            var viewModel = new ChangeToPaidEventViewModel
            {
                Id = entity.Id,
                Contact = entity.Contact,
                Costs = entity.Costs,
                CostsDescription = entity.CostsDescription,
                EventName = entity.Name,
                LastPaidDate = entity.LastSignINDateTime ?? Info.CentralEuropeNow,
                Link = entity.Link,
                Title = $"Převedení akce {entity.Name}"
            };

            AddBackButton(viewModel);

            return View(viewModel);
        }

        [Authorize(Roles = Strings.Administrator), ValidateAntiForgeryToken]
        [HttpPost, ActionName("ChangeToPaidEventEdit"), ValidateInput(false)]
        public ActionResult ChangeToPaidEventEditEdit(ChangeToPaidEventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var eventEntity = repository.One<Event>(model.Id);
                if (eventEntity.Price == 0)
                {
                    ModelState.AddModelError("", "Cena akce nesmí být nulová!");
                }

                if (eventEntity.Capacity > 0 && eventEntity.Costs > eventEntity.Capacity * eventEntity.Price)
                {
                    ModelState.AddModelError("", "Náklady jsou vyšší než možné příjmy z akce!");
                }

                if (ModelState.IsValid)
                {
                    var loginStatuses = new[] {EventLoginEnum.Prijdu, EventLoginEnum.Rezervace};

                    var participants =
                        eventEntity.EventParticipant.Where(x => loginStatuses.Contains(x.EventLoginStatus));

                    eventEntity.Link = model.Link;
                    eventEntity.CostsDescription = model.CostsDescription;
                    eventEntity.Contact = model.Contact;
                    eventEntity.Costs = model.Costs;
                    eventEntity.LastPaidDateTime = model.LastPaidDate;

                    foreach (var eventParticipant in participants)
                    {
                        logToEvent.ChangeParticipantStatus(eventEntity, eventParticipant.AspNetUsers,
                            EventLoginEnum.Rezervace, eventParticipant, false, eventParticipant.IsExternal);
                    }

                    eventEntity.EventType = EventTypeEnum.PlacenaSdruzenim;

                    repository.Save();


                    var recipients = participants
                        .Select(ep => ep.AspNetUsers)
                        .Distinct();

                    var entity = new Message()
                    {
                        AspNetUsers = GetSystemUser,
                        Id_Sender = GetSystemUser.Id,
                        Subject = $"Změna stavu akce: {eventEntity.Name}".TakeSafetely(50),
                        Text =
                            $"Akce {eventEntity.Name} se změnila na předplacenou akci. Pokud se chcete stále zúčastnit akce, je nutno znovu kliknout na tlačítko 'Přijdu'." +
                            $" </br>Děkuji za pochopení.",
                        Time = Info.CentralEuropeNow,
                        Visible = true,
                        Id_ReplyMessage = null,
                        MessageRecipient = recipients.Select(r => new MessageRecipient()
                        {
                            Id_User = Convert.ToInt32(r.Id),
                            Unreaded = true,
                            Visible = true,
                        }).ToArray(),
                    };

                    messageService.SendMessage(entity);

                    CreateToastrMessage("Akce byla v pořádku převedena.");

                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }
        #endregion

        #region Akce ke schválení
        public ViewResult EventToConfirm()
        {
            var events = repository.Where<Event>(e => e.State == EventStateEnum.ForAcceptance)
                .ToArray()
                .Select(e => new Tuple<string, string, int, DateTime>(e.Name, e.HtmlName, e.Id, e.StartDateTime))
                .OrderByDescending(e => e.Item3);

            var viewModel = new EnumerableBaseViewModel<Tuple<string, string, int, DateTime>>(events)
            {
                Title = "Akce ke schválení"
            };

            return View(viewModel);
        }
        #endregion

        #region Znovu poslat potvrzovací email
        [Authorize(Roles = Strings.Administrator)]
        public ActionResult PotvrzovaciEmail()
        {
            var model = new GenericBaseViewModel<string>("email")
            {
                Title = "Znovu poslání registračního emailu"
            };
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return View(model);
        }

        [Authorize(Roles = Strings.Administrator)]
        [HttpPost, ValidateAntiForgeryToken, ActionName("PotvrzovaciEmail")]
        public async Task<ActionResult> PotvrzovaciEmailPost(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                string code = await userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                var confirmUrl = Url.Action("ConfirmEmail", "Account", new {userId = user.Id, code},
                    Request.Url.Scheme);
                var data = new ConfirmUrlUserInformation()
                    {ConfirmUrl = confirmUrl, Email = user.Email, UserName = user.UserName};
                emailSender.SendEmail(EmailType.ConfirmNewUser, data, user.Email);
                logger.LogInfo($"Uživateli: {user.UserName} byl zaslán registrační email na {user.Email}",
                    "AdministraceController.PotvrzovaciEmailPost");

                CreateToastrMessage($"Pro uživatel {user.UserName} byl zaslán mail na {user.Email}");

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Uživatel nenalezen");

            var model = new GenericBaseViewModel<string>("email")
            {
                Title = "Znovu poslání registračního emailu"
            };
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");

            return View(model);
        }


        #endregion

        #region Stáhnutí stránky

        public ActionResult DownloadPage()
        {
            return View(GetDownloadViewModel());
        }

        [HttpPost, ActionName("DownloadPage")]
        public async Task<ActionResult> DowloadPagePost(string urlToDownload)
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                using (WebClient webClient = new WebClient())
                {
                    await webClient.DownloadStringTaskAsync(urlToDownload);
                    return View(GetDownloadViewModel(urlToDownload, true));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View(GetDownloadViewModel(e.Message));
            }
        }

        private GenericBaseViewModel<Tuple<bool, string>> GetDownloadViewModel(string message = "", bool success = false)
        {
            var model = new GenericBaseViewModel<Tuple<bool, string>>(new Tuple<bool, string>(success, message))
            {
                Title = "Stáhnutí stránky"
            };

            AddBackButton(model);

            return model;
        }
        #endregion

        #region Poslání info o akci

        public ActionResult PublishEvent()
        {
            var model = new EmptyBaseViewModel("Poslání info o akci");
            AddBackButton(model);
            return this.View(model);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName("PublishEvent")]
        public ActionResult PublishEventPost(string htmlname)
        {
            try
            {
                var entity = repository.OneByHtml<Event>(htmlname);
                var user = entity.EventOrganisator.First().AspNetUsers;
                eventUtils.Publicate(entity, user);

                this.CreateToastrMessage("Info o akci bylo posláno.");

                return this.RedirectToAction(string.Empty, "Administrace");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ex.Message);
                var model = new EmptyBaseViewModel("Poslání info o akci");
                AddBackButton(model);
                return this.View(model);
            }
        }
        #endregion

        private void AddBackButton(BaseViewModel model)
        {
            model.AddActionButton("Zpět na administraci", Utilities.Url("~/Administrace"), "icon-arrow-left");
        }
    }
}