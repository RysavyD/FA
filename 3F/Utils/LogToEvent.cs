using System;
using System.Linq;
using _3F.Log;
using _3F.Model;
using _3F.Model.Model;
using _3F.Model.Email;
using _3F.Model.Accounting;
using _3F.Model.Email.Model;
using _3F.Model.Extensions;
using _3F.Model.Service;
using _3F.Web.Models;

namespace _3F.Web.Utils
{
    public interface ILogToEvent
    {
        string LogToEvent(Event eventEntity, AspNetUsers user, EventLoginEnum newStatus, bool showActivity);
        void RefuseUser(EventParticipant participant);

        string LogExternalsToEvent(Event eventEntity, AspNetUsers user);
        string ConfirmExternalToEvent(EventParticipant externalParticipant);
        string CheckReservation(EventParticipant participant);
        void LogExternalsFromEvent(EventParticipant participant);
        void PaidPayment(Payment payment);
        EventParticipant ChangeParticipantStatus(Event eventEntity, AspNetUsers userEntity, EventLoginEnum newStatus,
            EventParticipant oldparticipant, bool showActivity, bool IsExternal = false);
    }

    public class LogToEvents : ILogToEvent
    {
        private ILogger logger;
        private IRepository repository;
        private IEmailSender emailSender;
        private IAccounting accounting;
        private IActivityListService activityListService;

        public LogToEvents(ILogger logger, IRepository repository, IEmailSender emailSender, IAccounting accounting, IActivityListService activityListService)
        {
            this.logger = logger;
            this.repository = repository;
            this.emailSender = emailSender;
            this.accounting = accounting;
            this.activityListService = activityListService;
        }

        public string LogToEvent(Event eventEntity, AspNetUsers user, EventLoginEnum newStatus, bool showActivity)
        {
            return AddUserToAction(eventEntity, user, newStatus, showActivity);
        }

        public void RefuseUser(EventParticipant participant)
        {
            LogFromEvent(participant.Event, participant.AspNetUsers, EventLoginEnum.Neprijdu, participant, false);
            SolveReserveParticipants(participant.Event, false);
        }

        public string LogExternalsToEvent(Event eventEntity, AspNetUsers user)
        {
            return LogUserToEvent(eventEntity, user, null, false, true);
        }

        public void LogExternalsFromEvent(EventParticipant participant)
        {
            LogFromEvent(participant.Event, participant.AspNetUsers, EventLoginEnum.Neprijdu, participant, false);
            SolveReserveParticipants(participant.Event, false); 
        }

        public string ConfirmExternalToEvent(EventParticipant externalParticipant)
        {
            return WorkFlow(externalParticipant.Event, externalParticipant.AspNetUsers, EventLoginEnum.Prijdu, externalParticipant, false);
        }

        /// <summary>
        /// Tvrdá logika (jádro) řešení přihlášení na akci (něco na styl workflow)
        /// </summary>
        private string AddUserToAction(Event eventEntity, AspNetUsers user, EventLoginEnum newStatus, bool showActivity)
        {
            var oldParticipant = eventEntity.EventParticipant.SingleOrDefault(ep => ep.Id_User == user.Id && !ep.IsExternal);
            if (eventEntity.IsPrivate && !eventEntity.UserHasInvivation(user))
                return string.Empty;

            return WorkFlow(eventEntity, user, newStatus, oldParticipant, showActivity);
        }

        private string WorkFlow(Event eventEntity, AspNetUsers user, EventLoginEnum newStatus, EventParticipant oldParticipant, bool showActivity)
        {
            EventLoginEnum oldLogin = (oldParticipant == null) ? EventLoginEnum.Nevyjadreno : oldParticipant.EventLoginStatus;
            if (showActivity)
            {
                user.DateLastActivity = Info.CentralEuropeNow;
                repository.Save();
            }

            #region Switch case WF
            switch (oldLogin)
            {
                case EventLoginEnum.Nevyjadreno:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        break;
                    }
                case EventLoginEnum.Prijdu:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            //nic
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            LogFromEvent(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            LogFromEvent(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        break;
                    }
                case EventLoginEnum.Mozna:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            //nic
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        break;
                    }
                case EventLoginEnum.Neprijdu:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            //nic
                        }
                        break;
                    }
                case EventLoginEnum.Nahradnik:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.NepotvrzenaRezervace)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.NepotvrzenaRezervace, oldParticipant, showActivity);                            
                        }
                        break;
                    }
                case EventLoginEnum.Rezervace:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            BreakUserPayment(oldParticipant.Id_Payment);
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            BreakUserPayment(oldParticipant.Id_Payment);
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        else if (newStatus == EventLoginEnum.RezervacePropadla)
                        {
                            BreakUserPayment(oldParticipant.Id_Payment);
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.RezervacePropadla, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        break;
                    }
                case EventLoginEnum.PoTerminu:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            //nic
                        }
                        break;
                    }
                case EventLoginEnum.RezervacePropadla:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        break;
                    }
                case EventLoginEnum.NepotvrzenaRezervace:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.RezervacePropadla)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.RezervacePropadla, oldParticipant, showActivity);
                            SolveReserveParticipants(eventEntity, showActivity); //metoda 3
                        }
                        break;
                    }
                case EventLoginEnum.Vyrizuji:
                    {
                        if (newStatus == EventLoginEnum.Prijdu)
                        {
                            return LogUserToEvent(eventEntity, user, oldParticipant, showActivity); //metoda 1
                        }
                        else if (newStatus == EventLoginEnum.Mozna)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Mozna, oldParticipant, showActivity);
                        }
                        else if (newStatus == EventLoginEnum.Neprijdu)
                        {
                            ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Neprijdu, oldParticipant, showActivity);
                        }
                        break;
                    }
            }
            #endregion
            return string.Empty;
        }

        /// <summary>
        /// Uživatel se chce přihlásit na akci -> skončí jako přihlášen, náhradník nebo rezervace (metoda 1)
        /// </summary>
        private string LogUserToEvent(Event eventEntity, AspNetUsers user, EventParticipant oldparticipant, bool showActivity, bool IsExternal = false)
        {
            /* Je na akci místo?
             *   NE - zapsat jako náhradník
             * ANO, je akce placená?
             *   NE - zapsat přihlášen
             * ANO, má uživatel dost peněz?
             *   NE - zapsat do rezervací a poslat mailem "fakturu"
             * ANO, zkusit strhnout peníze a zapsat jako přihlášen.  
             */
            // pocet prihlasenych uzivatelu
            int eventParticipantsCount = eventEntity.EventParticipant.Count(ep => (ep.EventLoginStatus == EventLoginEnum.Prijdu
                   || ep.EventLoginStatus == EventLoginEnum.Rezervace || ep.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace || ep.EventLoginStatus == EventLoginEnum.Vyrizuji) && ep != oldparticipant);

            if (eventEntity.Capacity <= eventParticipantsCount && eventEntity.Capacity > 0)
            {
                // na akci není volné místo
                if (oldparticipant != null && oldparticipant.EventLoginStatus == EventLoginEnum.Rezervace)
                {
                    // uživatel je už v rezervacích, tak jej znovu nepřihlašovat
                    return string.Empty;
                }
                // zapsat jako nahradnik
                if (oldparticipant == null || (oldparticipant != null && oldparticipant.EventLoginStatus != EventLoginEnum.Nahradnik))
                {
                    // nahradníka znovu mezi nahradníky nepsat
                    ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Nahradnik, oldparticipant, showActivity, IsExternal);
                }
                return string.Empty;
            }

            if (!eventEntity.IsPaidByOrganisation)
            {
                // akce není placená, tak normálně přihlásit
                ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Prijdu, oldparticipant, showActivity, IsExternal);
                return string.Empty;
            }

            oldparticipant = ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Vyrizuji, oldparticipant, false, IsExternal);

            // placená akce, jdou se řešit peníze, nejprve dotaz na stav konta uživatele
            AccountData data;
            if (user.VariableSymbol == null)
            {
                // uzivatel nema specificky symbol, zkusit jej zjistit z emailu a priradit, jinak informovat
                data = accounting.GetData(user.Email).Result;
                if (data == null || (data != null && data.VS == 0))
                {
                    logger.LogInfo("Uživatel nenalezen v platebním systému", "UserInAction.LogUserToAction");
                    var newSymbolData = accounting.GetNewUserSymbol(user.Email, user.UserName).Result;

                    if (newSymbolData != null && newSymbolData.Symbol != 0)
                    {
                        user.VariableSymbol = newSymbolData.Symbol;
                        logger.LogInfo(string.Format("Uživateli {0} přiřazen nový VS symbol: {1}", user.UserName, newSymbolData.Symbol), "UserInAction.LogUserToAction");
                    }
                    else
                    {
                        logger.LogInfo(string.Format("Od pokladny nepřiřazen VS pro uživatele {0}", user.UserName), "UserInAction.LogUserToAction");
                        ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Nevyjadreno, oldparticipant, false, IsExternal);
                        return "Chyba při přiřazování variabilního symbolu";
                    }
                }
                else
                {
                    user.VariableSymbol = data.VS;
                }
                repository.Save();
            }

            // nejprve se podívat, zda už neexistuje příkaz k platbě 
            var payment = oldparticipant.Payment;
            var paymentPaid = false;
            if (payment != null && payment.Status == PaymentStatus.Paid && payment.Amount < 0)
            {
                logger.LogDebug(string.Format("Uživatel se hlásí na akci a již má zaplacenou platbu, Uživatel:{0}, akce:{1}, Id_platba:{2}, Id_participant:{3}",
                    user.UserName,
                    eventEntity.Name,
                    payment.Id,
                    oldparticipant.Id),
                    "UserInAction.LogUserToAction");
                paymentPaid = true;
            }

            // platba neexistuje (první přihlášení nebo má kladnou částku = platba již byla vrácena při odhlášení a je nutno vytvořit novou
            if (payment == null || payment.Amount > 0)
            {
                try
                {
                    logger.LogDebug(string.Format("Přidávám novou platbu, Uživatel:{0}, akce:{1}", user.UserName, eventEntity.Name), "UserInAction.LogUserToAction");
                    payment = new Payment()
                    {
                        CreateDate = Info.CentralEuropeNow,
                        Id_User = user.Id,
                        Id_Event = eventEntity.Id,
                        Note = (IsExternal) ? string.Format("Platba za externistu na akci {0}", eventEntity.Name) : string.Format("Účast na akci {0}", eventEntity.Name),
                        Amount = (-1) * eventEntity.Price,
                        Status = PaymentStatus.Waiting,
                    };
                    repository.Add(payment);

                    oldparticipant.Payment = payment; // priradit platbu k prihlaseni
                    repository.Save();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ExceptionInfo(), "UserInAction.LogUserToAction");
                    return string.Format("Nastala chyba{0}{0}{1}", Environment.NewLine, ex.Message);
                }
            }

            MoveResult moveResult = MoveResult.EmptyResult;
            if (!paymentPaid)
            {
                try
                {
                    logger.LogDebug(string.Format("Provádění platby {0}, Uživatel:{1}, akce:{2}", payment.Id, user.UserName, eventEntity.Name), "UserInAction.LogUserToAction");
                    moveResult = accounting.MakeMove(payment.AspNetUsers.VariableSymbol.Value, payment.Amount, user.Email, payment.Event.AccountSymbol, payment.Note, payment.Id).Result;

                }
                catch (Exception ex)
                {
                    logger.LogException(ex, "UserInAction.LogUserToAction");
                    return "Nastal problém s platebním systémem";
                }
            }

            if (moveResult.CommunicationOk || paymentPaid)
            {
                // komunikace dopadla dobře, nastavit platbě správný status
                payment.Status = (moveResult.Paid) ? PaymentStatus.Paid : PaymentStatus.Active;
                repository.Save();

                if (moveResult.Paid || paymentPaid)
                {
                    if (!paymentPaid)
                    {
                        payment.UpdateDate = Info.CentralEuropeNow;
                        payment.Status = PaymentStatus.Paid;
                        // zaplaceno -> predat do ucastniku
                    }

                    ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Prijdu, oldparticipant, showActivity, IsExternal);
                }
                else
                {
                    // nezaplaceno (malo penez) -> do rezervaci                
                    ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Rezervace, oldparticipant, showActivity, IsExternal);
                    var emailModel = new PaymentInstruction()
                    {
                        Name = eventEntity.Name,
                        Price = eventEntity.Price,
                        VariableSymbol = user.VariableSymbol.Value,
                        EndReservationTime = Info.CentralEuropeNow.AddDays(5),
                    };

                    emailSender.SendEmail(EmailType.PaymentInstructions, emailModel, user.Email);
                }
            }
            else
            {
                // chyba komunikace -> platbu tak označit a uživatele do rezervace pro jistotu
                payment.Status = PaymentStatus.Waiting;
                ChangeParticipantStatus(eventEntity, user, EventLoginEnum.Rezervace, oldparticipant, showActivity, IsExternal);
                repository.Save();
            }

            return string.Empty;
        }

        /// <summary>
        /// Uživatel se odhlašuje z akce -> skončí jako možná, nepřijdu nebo odhlášen po termínu (metoda 2)
        /// </summary>
        private void LogFromEvent(Event eventEntity, AspNetUsers user, EventLoginEnum newStatus, EventParticipant oldparticipant, bool showActivity)
        {
            /*
             * Je uživatel odhlášen po termínu?
             *   ANO -> zapsat odhlášen po termínu
             * NE, zapsat nový status
             * Byla akce předplacená?
             *   ANO
             * Je před datumem splatnosti?
             *   ANO -> vrátit peníze (vytáhnout si je z plateb) kvůli možné změně ceny akce
             */
            if (eventEntity.LastSignINDateTime <= Info.CentralEuropeNow)
            {
                ChangeParticipantStatus(eventEntity, user, EventLoginEnum.PoTerminu, oldparticipant, showActivity);
                return;
            }
            ChangeParticipantStatus(eventEntity, user, newStatus, oldparticipant, showActivity);

            if (eventEntity.IsPaidByOrganisation)
            {
                if (eventEntity.LastPaidDateTime >= Info.CentralEuropeNow)
                {
                    // vratit platbu (pokud byla zaplacena) ... castka se bere z platby, kdyby se nahodou zmenila cena akce
                    Payment paidPayment = oldparticipant.Payment;

                    if (paidPayment == null)
                    {
                        logger.LogDebug($"Uživatel {oldparticipant.AspNetUsers.UserName} přihlášený na akci {oldparticipant.Event.Name} neměl přiřazenou platbu",
                            "UserInAction.LogFromEvent");
                        return;
                    }

                    if (paidPayment.Status == PaymentStatus.Paid)
                    {
                        var newPayment = new Payment()
                        {
                            Id_User = user.Id,
                            Id_Event = eventEntity.Id,
                            Note = (oldparticipant.IsExternal) ? string.Format("Odhlášení externisty z akce {0} v termínu", eventEntity.Name) : string.Format("Odhlášení z akce {0} v termínu", eventEntity.Name),
                            Amount = paidPayment.Amount * (-1),
                            CreateDate = Info.CentralEuropeNow,
                            Status = PaymentStatus.Waiting,
                        };
                        repository.Add(newPayment);

                        oldparticipant.Payment = newPayment; //ucast bude ukazovat na posledni platbu
                        repository.Save();

                        var payResult = accounting.MakeMove(
                            user.VariableSymbol.Value,
                            newPayment.Amount,
                            user.Email,
                            eventEntity.AccountSymbol,
                            newPayment.Note,
                            newPayment.Id).Result;

                        if (payResult.CommunicationOk)
                        {
                            if (payResult.Paid)
                                newPayment.UpdateDate = Info.CentralEuropeNow;

                            newPayment.Status = (payResult.Paid) ? PaymentStatus.Paid : PaymentStatus.Active;
                            repository.Save();
                        }
                    }
                    else if (paidPayment.Status == PaymentStatus.Active)
                    {
                        // stronovat aktivní platbu
                        paidPayment.Status = PaymentStatus.Cancelled;
                        paidPayment.UpdateDate = Info.CentralEuropeNow;
                        logger.LogInfo($"Stornuji platbu id:{paidPayment.Id}", "UserInAction.LogFromEvent");
                        repository.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Jsou-li na akce náhradníci, tak prvního převést do rezervace (metoda 3)
        /// </summary>
        private void SolveReserveParticipants(Event eventEntity, bool showActivity)
        {
            /* Jsou na akci nahradnici?
             * jestli ano, tak je na akci misto?
             * jestli ano, tak prvniho z nahradniku vzit a zmenit na rezervaci a poslat mu emaila
             * jinak nedelat nic
             */
            var participants = repository
                .Where<EventParticipant>(ep => ep.Id_Event == eventEntity.Id && ep.EventLoginStatus == EventLoginEnum.Nahradnik)
                .OrderBy(ep => ep.Time); // seradit podle casu prihlaseni, abychom byli spravedlivi


            if (participants == null || participants.Count() == 0)
                return; // nejsou náhradníci -> konec

            // pocet prihlasenych uzivatelu
            int eventParticipantsCount = eventEntity.EventParticipant.Count(i => i.EventLoginStatus == EventLoginEnum.Prijdu
                    || i.EventLoginStatus == EventLoginEnum.Rezervace);

            if (eventEntity.Capacity <= eventParticipantsCount)
                return; // na akci neni volne misto

            EventParticipant participant = participants.First(); // prvni z nahradniku
            ChangeParticipantStatus(eventEntity, participant.AspNetUsers, EventLoginEnum.NepotvrzenaRezervace, participant, showActivity);

            var emailModel = new NewReservation()
            {
                Name=eventEntity.Name,
                HtmlName = eventEntity.HtmlName,
                EndReservationTime = Info.CentralEuropeNow.AddDays(5),
            };

            emailSender.SendEmail(EmailType.FreePlaceOnEvent, emailModel, participant.AspNetUsers.Email);
        }

        /// <summary>
        /// Změní v databázi status přihlášení uživatele na akci, pokud neměl dosud status, tak jej vytvoří,jinak jej změní.
        /// </summary>
        public EventParticipant ChangeParticipantStatus(Event eventEntity, AspNetUsers userEntity, EventLoginEnum newStatus, EventParticipant oldparticipant, bool showActivity, bool IsExternal = false)
        {
            repository.Add(new EventParticipantHistory()
            {
                AspNetUsers = userEntity,
                Event = eventEntity,
                IsExternal = (oldparticipant == null) ? IsExternal : oldparticipant.IsExternal,
                NewEventLoginStatus = newStatus,
                OldEventLoginStatus = (oldparticipant == null) ? EventLoginEnum.Nevyjadreno : oldparticipant.EventLoginStatus,
                Time = Info.CentralEuropeNow,
                Id_Participant = (oldparticipant == null) ? 0 : oldparticipant.Id,
            });

            if (oldparticipant != null)
            {
                logger.LogInfo(string.Format("{0} změna statusu u akce {1} na {2} ze statusu {3}", userEntity.UserName, eventEntity.Name, newStatus, oldparticipant.EventLoginStatus), "UserInAction");
                oldparticipant.EventLoginStatus = newStatus;
                oldparticipant.Time = Info.CentralEuropeNow;
            }
            else
            {
                logger.LogInfo(string.Format("{0} nový status u akce {1} na {2}", userEntity.UserName, eventEntity.Name, newStatus), "UserInAction");
                oldparticipant = new EventParticipant()
                {
                    Id_Event = eventEntity.Id,
                    Id_User = userEntity.Id,
                    EventLoginStatus = newStatus,
                    Time = Info.CentralEuropeNow,
                    IsExternal = IsExternal,
                };
                repository.Add(oldparticipant);
            }
            repository.Save();

            if (showActivity && (newStatus == EventLoginEnum.Prijdu
                || newStatus == EventLoginEnum.Mozna
                || newStatus == EventLoginEnum.Nahradnik
                || newStatus == EventLoginEnum.Rezervace) && eventEntity.State == EventStateEnum.Active)
            {
                activityListService.AddActivity(ActivityCreator.Create(userEntity, eventEntity, ActivityType.LogToEvent));
            }
            return oldparticipant;
        }

        /// <summary>
        /// Zrusi prikaz k platbe uzivatele u dane akce. Napr. pokud se uzivatel odhlasuje z rezervaci.
        /// </summary>
        private void BreakUserPayment(int? idPayment)
        {
            if (idPayment.HasValue)
            {
                var payment = repository.One<Payment>(idPayment.Value);
                if (payment != null)
                {
                    logger.LogInfo(string.Format("zrušena platba s Id: {0}", payment.Id), "BreakUserPayment");
                    payment.Status = PaymentStatus.Cancelled;
                    payment.UpdateDate = Info.CentralEuropeNow;
                }

                // odstranit platbu ucastnika
                var particpipant = repository.One<EventParticipant>(ep => ep.Payment == payment);
                particpipant.Payment = null;

                repository.Save();
            }
        }

        public string CheckReservation(EventParticipant participant)
        {
            string result = string.Empty;
            logger.LogDebug(string.Format("Vyřizuji rezervaci č.{0}", participant.Id), "LogToEvents.CheckReservation");
            try
            {
                TimeSpan span = Info.CentralEuropeNow - participant.Time;
                if ((span.TotalHours >= 90 && participant.Event.EventType == EventTypeEnum.Bezna) ||
                    (span.TotalHours >= 120 && participant.Event.EventType == EventTypeEnum.PlacenaSdruzenim) ||
                    (span.TotalHours >= 120 && participant.Event.EventType == EventTypeEnum.Soukroma))
                {
                    //rezervace propadla casem, pokud nejsou nahradnici a rezervace visi do 240 hodin, nechat byt
                    int nahradnici = repository.Where<EventParticipant>
                        (p => p.Id_Event == participant.Id_Event && p.EventLoginStatus == EventLoginEnum.Nahradnik).Count();

                    if (nahradnici == 0 && span.TotalHours <= 240)
                    {
                        return string.Format("Rezervaci {0} ještě nepropadl čas", participant.Id);
                    }

                    LogToEvent(participant.Event, participant.AspNetUsers, EventLoginEnum.RezervacePropadla, false);
                    logger.LogInfo(string.Format("{0} propadla rezervace na akci {1}", participant.AspNetUsers.UserName, participant.Event.Name), "LogToEvents.CheckReservation");
                    result = string.Format("{0} propadla rezervace na akci {1}", participant.AspNetUsers.UserName, participant.Event.Name);

                    var emailModel = new NewReservation()
                    {
                        Name = participant.Event.Name,
                        HtmlName = participant.Event.HtmlName,
                    };
                    emailSender.SendEmail(EmailType.InfoAboutBreakReservation, emailModel, participant.AspNetUsers.Email);

                    if (participant.Event.EventType == EventTypeEnum.PlacenaSdruzenim)
                    {
                        //stornovat požadavek k platbě
                        Payment payment = participant.Payment;
                        if (payment != null && payment.Status == PaymentStatus.Active)
                        {
                            logger.LogInfo(string.Format("Zrušen požadavek na platbu {0} uživatele {1} k akci {2}", payment.Id, participant.AspNetUsers.UserName, participant.Event.Name), "LogToEvents.CheckReservation");
                            payment.Status = PaymentStatus.Cancelled;
                            participant.Payment = null;
                            repository.Save();
                        }
                    }
                }
                else
                {
                    result = string.Format("Rezervaci {0} ještě nepropadl čas", participant.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Při rušení rezervace {0}, nastala chyba {1}", participant.Id, ex.Message), "LogToEvents.CheckReservation");
                result = string.Format("Při rušení rezervace {0}, nastala chyba {1}", participant.Id, ex.Message);
            }
            repository.Save();

            return result;
        }

        public void PaidPayment(Payment payment)
        {
            var result = accounting.MakeMove(
              payment.AspNetUsers.VariableSymbol.Value,
              payment.Amount,
              payment.AspNetUsers.Email,
              payment.Event.AccountSymbol,
              payment.Note,
              payment.Id).Result;

            if (result.CommunicationOk && result.Paid)
            {
                payment.Status = PaymentStatus.Paid;
                payment.UpdateDate = Info.CentralEuropeNow;

                if (payment.Amount < 0)
                {
                    //když má uživatel zaplaceno (strženou částku), dostat ho z rezervací do účastníků
                    var participant = repository.One<EventParticipant>(ep => ep.Payment == payment);
                    if (participant != null && participant.EventLoginStatus == EventLoginEnum.Rezervace)
                    {
                        logger.LogInfo(string.Format("Uživatel {0} přesunut z rezervace do účasti na akci {1}",
                            participant.AspNetUsers.UserName,
                            participant.Event.HtmlName)
                            , "LogToEvent.PaidPAyment");

                        ChangeParticipantStatus(payment.Event, payment.AspNetUsers, EventLoginEnum.Prijdu, participant, false);
                    }
                    else
                    {
                        if (participant == null)
                        {
                            logger.LogInfo($"Nenalezen žádný účastník pro platbu {payment.Id}.", "LogToEvent.PaidPAyment");
                        }
                        else
                        {
                            logger.LogInfo($"Přihlášení Id:{participant.Id} není ve stavu rezervace, ale ve stavu '{participant.EventLoginStatus}', platba Id:{payment.Id}."
                                , "LogToEvent.PaidPAyment");
                        }
                    }
                }

                repository.Save();
            }
        }
    }
}