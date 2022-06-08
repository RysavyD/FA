using System.Collections.Generic;

namespace _3F.Web.Models
{
    public abstract class BaseViewModel
    {
        public string Icon { get; set; }
        public string BackgroundColor { get; set; }
        public string Title { get; set; }
        public List<ActionButton> Buttons { get; set; }

        protected BaseViewModel()
        {
            Buttons = new List<ActionButton>();
        }

        public BaseViewModel(string title) :this()
        {
            Title = title;
        }

        public void AddActionButton(string title, string url, string icon)
        {
            Buttons.Add(new ActionButton(title, url, icon));
        }

        public void AddActionButton(string title, string url, string icon, string @class, string id)
        {
            Buttons.Add(new ActionButton(title, url, icon, @class, id));
        }
    }

    public class ActionButton
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public string Class { get; set; }
        public string Id { get; set; }

        public ActionButton()
        { }

        public ActionButton(string title, string url, string icon)
        {
            Title = title;
            Url = url;
            Icon = icon;
        }

        public ActionButton(string title, string url, string icon, string @class, string id)
            : this(title, url, icon)
        {
            Class = @class;
            Id = id;
        }
    }

    public class EmptyBaseViewModel : BaseViewModel
    {
        public EmptyBaseViewModel() { }
        public EmptyBaseViewModel(string title) : base(title) { }
    }

    public class GenericBaseViewModel<T> : BaseViewModel where T: class
    {
        public GenericBaseViewModel(T entity)
        {
            Entity = entity;
        }

        public T Entity { get; set; }
    }

    public class EnumerableBaseViewModel<T> : BaseViewModel
    {
        public IEnumerable<T> Entities { get; set; }

        public EnumerableBaseViewModel()
        { }

        public EnumerableBaseViewModel(IEnumerable<T> entities)
        {
            Entities = entities;
        }
    }
}