using _3F.Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _3F.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Uživatelské jméno")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Uživatelské jméno")]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí bý alespoň {2} znaků dlouhé.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Heslo a potvrzení hesla nesouhlasí.")]
        public string ConfirmPassword { get; set; }

        public bool Agreement { get; set; }

        public int KnowFromId { get; set; }
        public SelectList KnowFroms { get; set; }

    }

    public class ChangePassword
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Staré heslo")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí bý alespoň {2} znaků dlouhé.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Heslo a potvrzení hesla nesouhlasí.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Heslo {0} musí bý alespoň {2} znaků dlouhé.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Heslo a potvrzení hesla nesouhlasí.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel : BaseViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginConfirmationViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Uživatelské jméno")]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public ExternalLoginConfirmationViewModel()
        {
            Title = "Registrace nového uživatele";
        }
    }

    public class EmailSendedViewModel : BaseViewModel
    {
        public string Email { get; set; }

        public EmailSendedViewModel()
        {
            Title = "Registrační email byl odeslán";
        }
    }
}