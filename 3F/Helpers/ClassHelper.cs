namespace _3F.Web.Helpers
{
    using System.Web.Mvc;

    public static class ClassHelper
    {
        public static void AddModelError(this ModelStateDictionary modelState, string message)
        {
            modelState.AddModelError(string.Empty, message);
        }
    }
}