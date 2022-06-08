using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using _3F.Model.Model;

namespace _3F.Web.Models.Administration
{
    public class LogUnit
    {
        public string DateTime { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }

        public LogUnit(string[] items)
        {
            DateTime = items[0];
            Level = items[1];
            Message = items[2];
            Action = items.Length > 3 ? items[3] : "";
        }

        public LogUnit(string line)
            : this(line.Split(new string[] { ";" }, StringSplitOptions.None))
        { }
    }

    public class LogName
    {
        public string FullName { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }

        public LogName(string fullName)
        {
            FullName = System.IO.Path.GetFileName(fullName);
            Name = System.IO.Path.GetFileNameWithoutExtension(FullName);
            Type = System.IO.Path.GetExtension(FullName).Replace(".log", "");
        }
    }

    public class UserWithRoles : User
    {
        public List<RoleViewModel> Roles { get; set; }
        public string Email { get; set; }

        public UserWithRoles()
        {
            Roles = new List<RoleViewModel>();
        }
    }

    public class RoleViewModel
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public bool IsInRole { get; set; }
        public bool IsEditable { get; set; }
        public override string ToString()
        {
            return RoleName;
        }
    }

    public class RoleWithUsers
    {
        public string Name { get; set; }
        public IEnumerable<User> Users { get; set; }
    }

    public class NewPaymentModel : BaseViewModel
    {
        public IEnumerable<SelectListItem> Events { get; set; }
        public string HtmlName { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
        public string UserName { get; set; }

        public IEnumerable<SelectListItem> Types { get; set; }
        public string Type { get; set; }

        public int Price { get; set; }

        public string Reason { get; set; }
    }

    public class UserWithSymbol : BaseViewModel
    {
        public string UserName { get; set; }
        public string Message { get; set; }

        public UserWithSymbol()
        {
            Title = "Zkontrolování VS uživatele";
        }
    }

    public class PaymentViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double Amount { get; set; }
        public string CreateDate { get; set; }
        public string EventName { get; set; }
        public string Note { get; set; }
        public string EventHtml { get; set; }
    }

    public class Reservation
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserHtmlName { get; set; }
        public string EventName { get; set; }
        public string EventHtmlName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Status { get; set; }
    }

    public class AdminEmail : BaseViewModel
    {
        [Required]
        public string Subject { get; set; }
        
        [Required]
        public string Body { get; set; }
    }

    public class NewsViewModel : BaseViewModel
    {
        public string News { get; set; }
    }

    public class ChangePaymentViewModel : BaseViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string EventName { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }

    public class ChangeEventParticipantModel : BaseViewModel
    {
        public int Id { get; set; }
        public string OldStatus { get; set; }
        public string UserName { get; set; }
        public string EventName { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }

    public class NewOrganisator
    {
        public User User { get; set; }
        public int EventCount { get; set; }
        public bool IsInRole { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public int EventCount { get; set; }
    }

    public class CategoryEdit : BaseViewModel
    {
        public string Name { get; set; }
    }

    public class EventBalance
    {
        public string Name { get; set; }
        public string HtmlName { get; set; }
        public string Organisators { get; set; }
        public int Costs { get; set; }
        public string CostsDescription { get; set; }
        public int Capacity { get; set; }
        public int Price { get; set; }
        public double Paid { get; set; }
        public double WillBePaid { get; set; }
        public string StartDate { get; set; }
    }
}
