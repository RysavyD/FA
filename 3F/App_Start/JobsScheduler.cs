using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using _3F.Log;
using _3F.Model;
using _3F.Model.Email;
using _3F.Model.Email.Model;
using _3F.Model.Extensions;
using _3F.Model.Model;
using _3F.Web.Utils;
using _3F.Model.Accounting;
using _3F.Model.Service;

namespace _3F.Web
{
    public class JobScheduler
    {
        public static void Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Repository>().As<IRepository>().InstancePerLifetimeScope();
            builder.RegisterType<Logger>().As<ILogger>();
            builder.RegisterType<FileChat>().As<IChat>();
            builder.RegisterType<ActivityListService>().As<IActivityListService>();

            builder.RegisterFromAppSettings<IAccounting>(false);
            builder.RegisterFromAppSettings<IEmailSender>(false);

            builder.RegisterType<LogToEvents>().As<ILogToEvent>();
            builder.RegisterType<EventUtils>().As<IEventUtils>();
            builder.RegisterType(typeof(ChatJob));
            builder.RegisterType(typeof(PeriodicEventsJob));
            builder.RegisterType(typeof(SendMayBeNoticesJob));
            builder.RegisterType(typeof(UnpaiedPaymentsJob));
            builder.RegisterType(typeof(BreakReservationJob));
            builder.RegisterType(typeof(ActivityJob));
            builder.RegisterType(typeof(FirstEventThanksJob));
            builder.RegisterType(typeof(Hydra2Job));

            var schedulerTask = Task.Run(async () => await StdSchedulerFactory.GetDefaultScheduler());
            schedulerTask.Wait();
            IScheduler scheduler = schedulerTask.Result;
            scheduler.JobFactory = new AutofacJobFactory(builder.Build());
            scheduler.Start();

            #region Job definitions
            IJobDetail chatJobDetail = JobBuilder.Create<ChatJob>().Build();
            ITrigger chatTrigger = TriggerBuilder.Create()
                .WithIdentity("chatJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(3, 30))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail periodicEventsJobDetail = JobBuilder.Create<PeriodicEventsJob>().Build();
            ITrigger periodicEventsTrigger = TriggerBuilder.Create()
                .WithIdentity("periodicEventsJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(3, 45))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail sendMayBeNoticesJobDetail = JobBuilder.Create<SendMayBeNoticesJob>().Build();
            ITrigger sendMayBeNoticesTrigger = TriggerBuilder.Create()
                .WithIdentity("sendMayBeNoticesJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4, 15))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail unpaiedPaymentsJobDetail = JobBuilder.Create<UnpaiedPaymentsJob>().Build();
            ITrigger unpaiedPaymentsTrigger = TriggerBuilder.Create()
                .WithIdentity("unpaiedPaymentsJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4, 30))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail breakReservationJobDetail = JobBuilder.Create<BreakReservationJob>().Build();
            ITrigger breakReservationTrigger = TriggerBuilder.Create()
                .WithIdentity("breakReservationJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4, 45))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail activityJobDetail = JobBuilder.Create<ActivityJob>().Build();
            ITrigger activityTrigger = TriggerBuilder.Create()
                .WithIdentity("activityJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4, 55))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail firstEventThanksDetail = JobBuilder.Create<FirstEventThanksJob>().Build();
            ITrigger firstEventThanksTrigger = TriggerBuilder.Create()
                .WithIdentity("firstEventThanksJob")
                .WithDailyTimeIntervalSchedule(
                    s => s.WithIntervalInHours(24)
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(4, 58))
                        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time")))
                .Build();

            IJobDetail hydra2Detail = JobBuilder.Create<Hydra2Job>().Build();
            ITrigger hydra2Trigger = TriggerBuilder.Create()
                .WithIdentity("hydra2Job")
                .WithCronSchedule(@"0 0/5 * * * ?") // every 5 minute
                .Build();
            #endregion

            scheduler.ScheduleJob(chatJobDetail, chatTrigger);
            scheduler.ScheduleJob(periodicEventsJobDetail, periodicEventsTrigger);
            scheduler.ScheduleJob(sendMayBeNoticesJobDetail, sendMayBeNoticesTrigger);
            scheduler.ScheduleJob(unpaiedPaymentsJobDetail, unpaiedPaymentsTrigger);
            scheduler.ScheduleJob(breakReservationJobDetail, breakReservationTrigger);
            scheduler.ScheduleJob(activityJobDetail, activityTrigger);
            scheduler.ScheduleJob(firstEventThanksDetail, firstEventThanksTrigger);
            scheduler.ScheduleJob(hydra2Detail, hydra2Trigger);
        }
    }

    [DisallowConcurrentExecution]
    public class ChatJob : IJob
    {
        readonly IChat _chat;
        readonly ILogger _logger;

        public ChatJob(IChat chat, ILogger logger)
        {
            _chat = chat;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Zapisuji svátek", "ChatJob");
            //_chat.AddMessage("!datum", "Main");
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class PeriodicEventsJob : IJob
    {
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEventUtils _eventUtils;
        const string NumberSearchPattern = @"(?<number>\d+)";

        public PeriodicEventsJob(ILogger logger, IRepository repository, IEventUtils eventUtils)
        {
            _logger = logger;
            _repository = repository;
            _eventUtils = eventUtils;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Spuštěno automatické vyhlašování", "PeriodicEventsJob");
            var periodicList = _repository.All<PeriodicEvent>().ToArray();
            foreach (var periodicEvent in periodicList)
            {
                if (IsPeriodicEventTime(periodicEvent))
                {
                    _logger.LogInfo(string.Format("Nalezena akce ve formátu {0}", periodicEvent.EventNameFormat), "PeriodicEventsJob");

                    try
                    {
                        int moveDays = DayMoveCount(periodicEvent);
                        string newName;
                        var oldEntity = GetLastOldEntity(periodicEvent.EventNameFormat, out newName);

                        var newEntity = _eventUtils.DuplicateEvent(oldEntity, moveDays, newName);
                        newEntity.State = EventStateEnum.Active;
                        _repository.Add(newEntity);
                        _eventUtils.WorkFlow(newEntity, null);

                        // before publicating set actual context
                        var request = new System.Web.HttpRequest("/", System.Configuration.ConfigurationManager.AppSettings["MainUrl"], "");
                        var response = new System.Web.HttpResponse(new System.IO.StringWriter());
                        System.Web.HttpContext.Current = new System.Web.HttpContext(request, response);

                        _eventUtils.Publicate(newEntity, null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogException(ex, "PeriodicEventsJob");
                    }
                }
            }

            _logger.LogInfo("Automatické vyhlašování dokončeno", "PeriodicEventsJob");
            return Task.CompletedTask;;
        }

        private bool IsPeriodicEventTime(PeriodicEvent eventEntity)
        {
            DateTime now = Info.CentralEuropeNow;
            switch (eventEntity.PeriodicEventType)
            {
                case PeriodicEventTypeEnum.Daily:
                    return true;
                case PeriodicEventTypeEnum.DayInMonth:
                    return (now.Day == eventEntity.PeriodicParameter);
                case PeriodicEventTypeEnum.DayInWeek:
                    return ((int)now.DayOfWeek) == eventEntity.PeriodicParameter;
                case PeriodicEventTypeEnum.Monthly:
                    return (now.Day == 1);
                case PeriodicEventTypeEnum.Weekly:
                    return (now.DayOfWeek == DayOfWeek.Monday);
                case PeriodicEventTypeEnum.Yearly:
                    return (now.Day == 1 && now.Month == 1);
                default:
                    return false;
            }
        }

        private int DayMoveCount(PeriodicEvent eventEntity)
        {
            DateTime now = Info.CentralEuropeNow;

            switch (eventEntity.PeriodicEventType)
            {
                case PeriodicEventTypeEnum.Daily:
                    return 1;
                case PeriodicEventTypeEnum.DayInMonth:
                    return DateTime.DaysInMonth(now.Year, now.Month);
                case PeriodicEventTypeEnum.DayInWeek:
                    return 7;
                case PeriodicEventTypeEnum.Monthly:
                    return DateTime.DaysInMonth(now.Year, now.Month);
                case PeriodicEventTypeEnum.Weekly:
                    return 7;
                case PeriodicEventTypeEnum.Yearly:
                    return new DateTime(now.Year, 12, 31).DayOfYear;
                default:
                    return 0;
            }
        }

        private Event GetLastOldEntity(string eventNameFormat, out string newName)
        {
            var regex = new Regex(eventNameFormat);
            string[] eventNames = _repository
                .Where<Event>(ev => ev.State == EventStateEnum.Active || ev.State == EventStateEnum.Deleted)
                .Select(ev => ev.Name)
                .ToArray();

            int max = 0;
            foreach (var eventName in eventNames)
            {
                var match = regex.Match(eventName);
                if (match.Success)
                    max = Math.Max(max, Convert.ToInt32(match.Groups["number"].Value));
            }

            string oldEntityName = eventNameFormat.Replace(NumberSearchPattern, max.ToString());
            var eventEntity = _repository
                .Where<Event>(ev => ev.Name == oldEntityName)
                .OrderByDescending(s => s.Id)
                .First();
            newName = eventNameFormat.Replace(NumberSearchPattern, (++max).ToString());

            return eventEntity;
        }
    }

    [DisallowConcurrentExecution]
    public class SendMayBeNoticesJob : IJob
    {
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEmailSender _emailSender;

        public SendMayBeNoticesJob(ILogger logger, IRepository repository, IEmailSender emailSender)
        {
            _logger = logger;
            _repository = repository;
            _emailSender = emailSender;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Spuštěno upozornění na přihlášení", "SendMayBeNotices");

            var nextTwoDayDate = Info.CentralEuropeNow.Date.AddDays(2);
            var nextThreeDayDate = nextTwoDayDate.AddDays(1);

            var events = _repository
                .Where<Event>(ev => nextTwoDayDate <= ev.StartDateTime && ev.StartDateTime < nextThreeDayDate && ev.State == EventStateEnum.Active)
                .Select(ev => new EvenWithtParticipantsEmailModel()
                {
                    EventId = ev.Id,
                    Name = ev.Name,
                    HtmlName = ev.HtmlName,
                    Perex = ev.Perex,
                })
                .ToArray();

            foreach (var entity in events)
            {
                var entityId = entity.EventId;
                var maybeEmails = _repository.Where<EventParticipant>(p => p.Id_Event == entityId && p.EventLoginStatus == EventLoginEnum.Mozna)
                    .Where(p => p.AspNetUsers.Profiles.SendMayBeEventNotice)
                    .Select(p => p.AspNetUsers.Email)
                    .ToArray();
                var yesEmails = _repository.Where<EventParticipant>(p => p.Id_Event == entityId && p.EventLoginStatus == EventLoginEnum.Prijdu)
                    .Where(p => p.AspNetUsers.Profiles.SendMayBeEventNotice)
                    .Select(p => p.AspNetUsers.Email)
                    .ToArray();

                _logger.LogInfo(string.Format("U akce {0} zasílám upozornění.", entity.Name), "SendMayBeNotices");
                _emailSender.SendEmail(EmailType.EventMayBeNotice, entity, maybeEmails);
                _emailSender.SendEmail(EmailType.EventYesNotice, entity, yesEmails);
            }

            _logger.LogInfo("Dokončeno upozorňování na přihlášení Možná", "SendMayBeNotices");
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class UnpaiedPaymentsJob : IJob
    {
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly ILogToEvent _logToEvent;

        public UnpaiedPaymentsJob(ILogger logger, IRepository repository, ILogToEvent logToEvent)
        {
            _logger = logger;
            _repository = repository;
            _logToEvent = logToEvent;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Spuštěno automatické placení plateb", "UnpaiedPaymentJob");

            var paymentIds = _repository
                .Where<Payment>(p => p.Status == PaymentStatus.Active)
                .Select(p => p.Id)
                .ToArray();
            foreach (var id in paymentIds)
            {
                var payment = _repository.One<Payment>(id);
                _logger.LogInfo("Kontroluji platbu " + payment.Id, "UnpaiedPaymentJob");
                try
                {
                    _logToEvent.PaidPayment(payment);
                    _logger.LogInfo(
                        string.Format("Platba byla vykonána. Stav platby je {0}", payment.Status.GetDescription()),
                        "UnpaiedPaymentJob");
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, "UnpaiedPaymentJob");
                }
            }

            _logger.LogInfo("Dokončeno automatické placení plateb", "UnpaiedPaymentJob");
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class BreakReservationJob : IJob
    {
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly ILogToEvent _logToEvent;

        public BreakReservationJob(ILogger logger, IRepository repository, ILogToEvent logToEvent)
        {
            _logger = logger;
            _repository = repository;
            _logToEvent = logToEvent;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Spuštěna automatická kontrola rezervací", "BreakReservationJob");
            var participantIds = _repository.Where<EventParticipant>
                (ev =>
                    ev.EventLoginStatus == EventLoginEnum.Rezervace ||
                    ev.EventLoginStatus == EventLoginEnum.NepotvrzenaRezervace)
                .Where(ev => ev.Event.StartDateTime >= Info.CentralEuropeNow)
                .OrderByDescending(ev => ev.Time)
                .Select(ev => ev.Id)
                .ToArray();

            foreach (var id in participantIds)
            {
                try
                {
                    var participant = _repository.One<EventParticipant>(id);
                    _logger.LogInfo("Kontroluji rezervaci " + participant.Id, "BreakReservationJob");
                    string result = _logToEvent.CheckReservation(participant);

                    _logger.LogInfo(
                        string.Format("Rezervace byla zkontrolována s výsledkem: {0}", result),
                        "BreakReservationJob");
                }
                catch (Exception ex)
                {
                    _logger.LogException(ex, "BreakReservationJob");
                }
            }

            _logger.LogInfo("Dokončena automatická kontrola rezervací", "BreakReservationJob");
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class ActivityJob : IJob
    {
        private readonly IActivityListService _activityListService;
        readonly ILogger _logger;

        public ActivityJob(IActivityListService activityListService, ILogger logger)
        {
            _activityListService = activityListService;
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Archivuji aktivity", "ActivityJob");
            _activityListService.ArchiveActivities(100);
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class FirstEventThanksJob : IJob
    {
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEmailSender _emailSender;

        public FirstEventThanksJob(ILogger logger, IRepository repository, IEmailSender emailSender)
        {
            _logger = logger;
            _repository = repository;
            _emailSender = emailSender;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Spuštěno poděkování za první akci", "FirstEventThanks");

            var today = Info.CentralEuropeNow.Date;
            var startDate = today.AddDays(-3);
            var stopDate = startDate.AddDays(1);

            var users = _repository
                .Where<Event>(ev => ev.StopDateTime >= startDate && ev.StopDateTime <= stopDate && ev.State == EventStateEnum.Active)
                .SelectMany(ev => ev.EventParticipant)
                .Where(ep => ep.EventLoginStatus == EventLoginEnum.Prijdu)
                .Select(ep => ep.AspNetUsers)
                .Distinct();

            foreach (var user in users)
            {
                var eventCount = user.EventParticipant
                     .Where(ep => ep.EventLoginStatus == EventLoginEnum.Prijdu)
                     .Select(ep => ep.Event)
                     .Count(ev => ev.State == EventStateEnum.Active && ev.StopDateTime <= today);

                if (eventCount == 1)
                {
                    _logger.LogInfo($"Posláno poděkování uživatel {user.UserName}", "FirstEventThanks");
                    _emailSender.SendEmail(EmailType.ThanksForFirstEvent, user, user.Email);
                }
            }

            _logger.LogInfo("Dokončeno  poděkování za první akci", "FirstEventThanks");
            return Task.CompletedTask;;
        }
    }

    [DisallowConcurrentExecution]
    public class Hydra2Job : IJob
    {
        readonly ILogger _logger;

        public Hydra2Job(ILogger logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Dotazuji hydru", "Hydra2Job");
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadString("http://hydra2.dusanrysavy.cz/Graf");
                }

                _logger.LogInfo("Ok", "Hydra2Job.Execute");
            }
            catch (Exception e)
            {
                _logger.LogException(e, "Hydra2Job.Execute");
            }
            return Task.CompletedTask;;
        }
    }

    public class AutofacJobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public AutofacJobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_container.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
        }
    }

}