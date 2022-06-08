using System.Linq;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Utils;
using _3F.Model.Service.Model;

namespace _3F.Web.Models
{
    public enum ActivityType
    {
        CommentDiscussion = 0,
        LogToEvent = 1,
        Registration = 2,
        CreateEvent = 3,
        EditEvent = 4,
        CreateSummary = 5,
        EditSummary = 6,
        CreateDiscussion = 7,
        DeleteEvent = 8,
        CreatePhotoAlbum = 9,
        ConfirmEvent = 10,
        CreateSuggestedEvent = 11,
    }

    public class ActivityCreator
    {
        public static ActivityModel Create(AspNetUsers user, Model.Model.Discussion discussion, ActivityType type)
        {
            if (type == ActivityType.CommentDiscussion)
            {
                if (discussion.Event.Any())
                {
                    if (discussion.Event.First().IsPrivate)
                        return null;

                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> komentoval(a) <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", discussion.Event.First().HtmlName)
                            , discussion.Event.First().Name),
                    };
                }

                if (discussion.EventSummary.Any())
                {
                    if (discussion.EventSummary.First().Event.IsPrivate)
                        return null;

                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> komentoval(a) <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Zapis/Detail/", discussion.EventSummary.First().Event.HtmlName)
                            , discussion.EventSummary.First().Name),
                    };
                }

                if (discussion.PhotoAlbum.Any())
                {
                    if (discussion.PhotoAlbum.First().Event.IsPrivate)
                        return null;

                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> komentoval(a) <a href=\"{2}\">album</a> k akci {3}"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Fotky/Album/", discussion.PhotoAlbum.First().Id.ToString())
                            , discussion.PhotoAlbum.First().Event.Name),
                    };
                }

                return new ActivityModel()
                {
                    User = new Model.Service.Model.User(user),
                    Time = Info.CentralEuropeNow,
                    Text = string.Format("<a href=\"{0}\">{1}</a> komentoval(a) diskuzi <a href=\"{2}\">{3}</a>"
                        , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                        , user.UserName
                        , Utilities.Url("~/Diskuze/Detail/", discussion.HtmlName)
                        , discussion.Name),
                };
            }
            else if (type == ActivityType.CreateDiscussion)
            {
                return new ActivityModel()
                {
                    User = new Model.Service.Model.User(user),
                    Time = Info.CentralEuropeNow,
                    Text = string.Format("<a href=\"{0}\">{1}</a> založil(a) diskuzi <a href=\"{2}\">{3}</a>"
                        , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                        , user.UserName
                        , Utilities.Url("~/Diskuze/Detail/", discussion.HtmlName)
                        , discussion.Name),
                };
            }

            return null;
        }

        public static ActivityModel Create(AspNetUsers user, Event eventEntity, ActivityType type)
        {
            if (eventEntity.IsPrivate)
                return null;

            switch (type)
            {
                case ActivityType.LogToEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> se přihlásil(a) na akci <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                            , eventEntity.Name),
                    };
                case ActivityType.CreateEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> vytvořil(a) akci <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                            , eventEntity.Name),
                    };
                case ActivityType.EditEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> editoval(a) akci <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                            , eventEntity.Name),
                    };
                case ActivityType.DeleteEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> smazal(a) akci <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                            , eventEntity.Name),
                    };
                case ActivityType.ConfirmEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> schválil(a) akci <a href=\"{2}\">{3}</a>"
                            , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                            , user.UserName
                            , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                            , eventEntity.Name),
                    };
                case ActivityType.CreateSuggestedEvent:
                    return new ActivityModel()
                    {
                        User = new Model.Service.Model.User(user),
                        Time = Info.CentralEuropeNow,
                        Text = string.Format("<a href=\"{0}\">{1}</a> vytvořil(a) nápad na akci <a href=\"{2}\">{3}</a>"
                        , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                        , user.UserName
                        , Utilities.Url("~/Akce/Detail/", eventEntity.HtmlName)
                        , eventEntity.Name),
                    };
            }

            return null;
        }

        public static ActivityModel Create(AspNetUsers user, EventSummary summary, ActivityType type)
        {
            if (type == ActivityType.CreateSummary && !summary.Event.IsPrivate)
            {
                return new ActivityModel()
                {
                    User = new Model.Service.Model.User(user),
                    Time = Info.CentralEuropeNow,
                    Text = string.Format("<a href=\"{0}\">{1}</a> vytvořil(a) <a href=\"{2}\">{3}</a>"
                        , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                        , user.UserName
                        , Utilities.Url("~/Zapis/Detail/", summary.Event.HtmlName)
                        , summary.Name),
                };
            }
            else if (type == ActivityType.EditSummary && !summary.Event.IsPrivate)
            {
                return new ActivityModel()
                {
                    User = new Model.Service.Model.User(user),
                    Time = Info.CentralEuropeNow,
                    Text = string.Format("<a href=\"{0}\">{1}</a> editoval(a) <a href=\"{2}\">{3}</a>"
                        , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                        , user.UserName
                        , Utilities.Url("~/Zapis/Detail/", summary.Event.HtmlName)
                        , summary.Name),
                };
            }

            return null;
        }

        public static ActivityModel Create(AspNetUsers user, PhotoAlbum album)
        {
            if (album.Event.IsPrivate)
                return null;

            return new ActivityModel()
            {
                User = new Model.Service.Model.User(user),
                Time = Info.CentralEuropeNow,
                Text = string.Format("<a href=\"{0}\">{1}</a> vytvořil(a) <a href=\"{2}\">galerii</a> k akci {3}"
                    , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                    , user.UserName
                    , Utilities.Url("~/Fotky/Album/", album.Id.ToString())
                    , album.Event.Name)
            };
        }

        public static ActivityModel Create(ApplicationUser user)
        {
            return new ActivityModel()
            {
                User = new Model.Service.Model.User()
                {
                    id = user.Id.ToString(),
                    htmlName = user.HtmlName,
                    name = user.UserName,
                    ProfilePhoto = user.ProfilePhoto,
                },
                Time = Info.CentralEuropeNow,
                Text = string.Format("<a href=\"{0}\">{1}</a> se právě zaregistroval(a)."
                    , Utilities.Url("~/Profil/Detail/", user.HtmlName)
                    , user.UserName),
            };
        }
    }
}