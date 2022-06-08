namespace _3F.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using _3F.Log;
    using _3F.Model;
    using _3F.Model.Email;
    using _3F.Model.Email.Model;
    using _3F.Model.Extensions;
    using _3F.Model.Model;
    using _3F.Model.Repositories;
    using _3F.Model.Service;
    using _3F.Web.Helpers;
    using _3F.Web.Models;
    using _3F.Web.Utils;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    [Authorize]
    public class AccountController : BaseController
    {
        private IAuthenticationManager authenticationManager;
        private ApplicationUserManager userManager;
        private ApplicationSignInManager signInManager;
        private IEmailSender emailSender;
        private IActivityListService activityListSevice;
        private IKnowFromRepository knowFromRepository;

        public AccountController(
            IAuthenticationManager authenticationManager,
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IEmailSender emailSender,
            IActivityListService activityListSevice,
            IKnowFromRepository knowFromrepository)
        {
            this.authenticationManager = authenticationManager;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.activityListSevice = activityListSevice;
            this.knowFromRepository = knowFromrepository;

            this.Icon = string.Empty;
            this.BackgroundColor = "orange-background"; 
        }

        #region Login
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return this.View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            ApplicationUser user = await this.userManager.FindByNameAsync(model.UserName) ??
                                    await this.userManager.FindByEmailAsync(model.UserName);

            if (user == null)
            {
                ModelState.AddModelError("Neexistující uživatel.");
                ModelState.AddModelError("Zkontrolujte v uživatelském jménu velká a malá písmena.");
                return this.View(model);
            }

            switch (user.LoginType)
            {
                case LoginTypeEnum.OldSystemConfirmed:
                    var oldUser = repository.Where<OldPassword>(o => o.Id_User == user.Id).First();
                    if (Utilities.Validate(oldUser, model.Password))
                    {
                        this.authenticationManager.SignOut();
                        await this.signInManager.SignInAsync(user, true, true);
                        user.DateLastActivity = Info.CentralEuropeNow;
                        await this.userManager.UpdateAsync(user);
                        logger.LogInfo($"{user.UserName} se přihlásil z adresy {Request.UserHostAddress}", "AccountController.Login");
                        return this.RedirectToLocal(returnUrl);
                    }

                    ModelState.AddModelError("Uživatelské jméno nebo heslo je nesprávné.");
                    logger.LogInfo($"Neplatné přihlášení username:{model.UserName}", "AccountController.Login");
                    return this.View(model);
                case LoginTypeEnum.OldSystemNotConfirmed:
                case LoginTypeEnum.NotConfirmed:
                    ModelState.AddModelError("Uživatelský účet nebyl ještě potvrzen.");
                    return this.View(model);

                case LoginTypeEnum.Deleted:
                    ModelState.AddModelError("Uživatelský účet je smazaný.");
                    return this.View(model);

                case LoginTypeEnum.Blocked:
                    ModelState.AddModelError("Uživatelský účet je zablokovaný.");
                    return this.View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await this.signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

            switch (result)
            {
                case SignInStatus.Success:
                    {
                        logger.LogInfo($"{user.UserName} se přihlásil z adresy {Request.UserHostAddress}", "AccountController.Login");
                        user.DateLastActivity = Info.CentralEuropeNow;
                        await this.userManager.UpdateAsync(user);
                        return this.RedirectToLocal(returnUrl);
                    }

                default:
                    {
                        logger.LogInfo($"Neplatné přihlášení username:{model.UserName}", "AccountController.Login");
                        ModelState.AddModelError("Uživatelské jméno nebo heslo je nesprávné.");
                        return this.View(model);
                    }
            }
        }
        #endregion

        #region Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new RegisterViewModel();
            this.FillRegisterModel(model);

            return this.View(model);
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserName.Contains("@"))
                {
                    ModelState.AddModelError("Uživatelské jméno obsahuje nepovolené znaky.");
                }

                if (!model.Agreement)
                {
                    ModelState.AddModelError("Je třeba souhlasit s podmínkami užití.");
                }

                if (repository.All<AspNetUsers>().Select(u => u.UserName.ToLower())
                    .ToArray()
                    .Select(name => name.RemoveDiakritics())
                    .Contains(model.UserName.ToLower().RemoveDiakritics()))
                {
                    ModelState.AddModelError("Uživatelské jméno už je použito.");
                }

                var blockedAdresses = repository.Where<ProgramSetting>(ps => ps.Key == "BlockRegistrationIP");
                if (blockedAdresses.Any(b => b.Value == Request.UserHostAddress))
                {
                    ModelState.AddModelError($"Vaše ip adresa {Request.UserHostAddress} je blokována pro registraci.");
                }

                if (!ModelState.IsValid)
                {
                    this.FillRegisterModel(model);
                    return this.View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Profile = this.CreateDefaultProfile(),
                    RegisterType = model.KnowFromId,
                };

                var result = await this.userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    this.repository.SetHtmlName(repository.One<AspNetUsers>(u => u.UserName == model.UserName), model.UserName);
                    this.repository.Save();

                    string code = await this.userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                    var data = new ConfirmUrlUserInformation() { ConfirmUrl = confirmUrl, Email = model.Email, UserName = model.UserName };
                    this.emailSender.SendEmail(EmailType.ConfirmNewUser, data, model.Email);
                    logger.LogInfo($"Uživatel: {model.UserName} byl právě vytvořen, IP požadavku {Request.UserHostAddress}", "AccountController.Register");

                    return this.View("EmailSended", new EmailSendedViewModel() { Email = model.Email });
                }

                this.AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            this.FillRegisterModel(model);
            return this.View(model);
        }

        private Profile CreateDefaultProfile()
        {
            return new Profile()
            {
                SendMayBeEventNotice = true,
                SendMessagesFromAdminToMail = true,
                SendMessagesToMail = true,
                SendNewActionToMail = true,
                SendNewAlbumsToMail = true,
                SendNewSummaryToMail = true,
            };
        }

        private void FillRegisterModel(RegisterViewModel model)
        {
            model.KnowFroms = new SelectList(this.knowFromRepository.GetAll().Where(k => k.Visible), "Id", "Text", 1);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(int? userId, string code)
        {
            if (userId == null || string.IsNullOrWhiteSpace(code))
            {
                logger.LogDebug($"user Id:'{userId}', code:'{code}'", "Account.ConfirmEmail");
                return this.View("Error");
            }

            var user = await userManager.FindByIdAsync(userId.Value);
            if (user != null && user.LoginType == LoginTypeEnum.Deleted)
                return this.View("Error");

            var result = await this.userManager.ConfirmEmailAsync(userId.Value, code);
            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                user.LoginType = LoginTypeEnum.Confirmed;

                // přidělit novému uživateli číslo od pokladního systému
                // var newSysmbol = await accounting.GetNewUserSymbol(user.Email, user.UserName);
                // if (string.IsNullOrEmpty(newSysmbol.Problem) && newSysmbol.Symbol > 0)
                //    user.VariableSymbol = newSysmbol.Symbol;
                // přiděluje se při prvním přihlášení na placenou akci!
                this.userManager.Update(user);
                await this.signInManager.SignInAsync(user, true, true);
                this.CopyImageFiles(user.HtmlName);

                // do aktivit přidat, že se zaregistroval nový uživatel
                this.activityListSevice.AddActivity(ActivityCreator.Create(user));

                logger.LogInfo($"Uživatel {user.UserName} se právě zaregistroval", "AccountController.ConfirmEmail");
                return this.View("ConfirmEmail", new EmptyBaseViewModel() { Title = "Registrace dokončena" });
            }

            logger.LogDebug($"user Id:'{userId}', code:'{code}', result:'{result}'", "Account.ConfirmEmail");
            return this.View("Error");
        }
        #endregion

        #region LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            this.authenticationManager.SignOut();
            return this.RedirectToAction("Index", "Home");
        }
        #endregion

        #region Forgot password
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return this.View(new ForgotPasswordViewModel() { Title = "Zapomenuté heslo" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(model.Email);
                if (user != null && user.LoginType == LoginTypeEnum.Deleted)
                {
                    ModelState.AddModelError("Uživatel je smazán");
                    return this.View(model);
                }

                if ((user != null) && (await this.userManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await this.userManager.GeneratePasswordResetTokenAsync(user.Id);
                    var confirmUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, Request.Url.Scheme);
                    var data = new ConfirmUrlUserInformation() { ConfirmUrl = confirmUrl, Email = model.Email, UserName = user.UserName };
                    this.emailSender.SendEmail(EmailType.ForgotPassword, data, model.Email);
                    logger.LogInfo($"Uživatel: {user.UserName} si žádá o zapomenuté heslo", "AccountController.ForgotPassword");

                    return this.RedirectToAction("ForgotPasswordConfirmation", "Account");
                }

                // Don't reveal that the user does not exist or is not confirmed
                this.ModelState.AddModelError("Uživatel neexistuje");
            }

            model.Title = "Zapomenuté heslo";

            // If we got this far, something failed, redisplay form
            return this.View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return this.View(new EmptyBaseViewModel() { Title = "Email odeslán" });
        }
        #endregion

        #region Reset password
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? this.View("Error") : this.View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("Uživatel neexistuje");
                return this.View(model);
            }

            if (user.LoginType == LoginTypeEnum.Deleted)
            {
                ModelState.AddModelError("Uživatel je smazán");
                return this.View(model);
            }

            var resetRresult = await this.userManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            var changeResult = await this.userManager.ChangePasswordAsync(user.Id, model.Password, model.Password);
                        
            if (resetRresult.Succeeded && changeResult.Succeeded)
            {
                user.LoginType = LoginTypeEnum.Confirmed;
                user.DateLastActivity = Info.CentralEuropeNow;
                await this.userManager.UpdateAsync(user);
                return this.RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            this.AddErrors(resetRresult);
            this.AddErrors(changeResult);

            return this.View();
        }
        
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return this.View();
        }
        #endregion

        #region External login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            logger.LogDebug($"provider: {provider}, returnUrl:{returnUrl}", "Account.ExternalLogin");
            ControllerContext.HttpContext.Session.RemoveAll();

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }), this.authenticationManager, logger);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            logger.LogDebug($"returnUrl:{returnUrl}", "Account.ExternalLoginCallback");
            var loginInfo = await this.authenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                logger.LogDebug("loginInfo is null", "Account.ExternalLoginCallback");
                return this.RedirectToAction("Login");
            }

            logger.LogDebug($"DefaultUserName:{loginInfo.DefaultUserName}, Email:{loginInfo.Email}, Login:{loginInfo.Login}", "Account.ExternalLoginCallback");

            // Sign in the user with this external login provider if the user already has a login
            var result = await this.signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            logger.LogDebug($"result:{result}", "Account.ExternalLoginCallback");

            switch (result)
            {
                case SignInStatus.Success:
                    return this.RedirectToLocal(returnUrl);
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return this.View(
                        "ExternalLoginConfirmation",
                        new ExternalLoginConfirmationViewModel
                        {
                            UserName = loginInfo.DefaultUserName,
                            Email = loginInfo.Email,
                        });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                logger.LogDebug("User is authenticate, redirect to Manage/Index", "Account.ExternalLoginConfirmation");
                return this.RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                if (model.UserName.Contains("@"))
                {
                    ModelState.AddModelError("Uživatelské jméno obsahuje nepovolené znaky.");
                }

                if (repository.All<AspNetUsers>().Select(u => u.UserName.ToLower())
                    .ToArray()
                    .Select(name => name.RemoveDiakritics())
                    .Contains(model.UserName.ToLower().RemoveDiakritics()))
                {
                    ModelState.AddModelError("Uživatelské jméno už je použito.");
                }
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.authenticationManager.GetExternalLoginInfoAsync();
                logger.LogDebug("info " + info, "Account.ExternalLoginConfirmation");

                if (info == null)
                {
                    return this.View("ExternalLoginFailure");
                }

                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    LoginType = LoginTypeEnum.Confirmed,
                    EmailConfirmed = true,
                    Profile = this.CreateDefaultProfile(),
                };

                var result = await this.userManager.CreateAsync(user);
                logger.LogDebug("result " + result, "Account.ExternalLoginConfirmation");

                if (result.Succeeded)
                {
                    var entityUser = repository.One<AspNetUsers>(u => u.UserName == user.UserName);
                    user.HtmlName = repository.SetHtmlName(entityUser, user.UserName);

                    this.CopyImageFiles(user.HtmlName);

                    result = await this.userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        this.activityListSevice.AddActivity(ActivityCreator.Create(user));
                        logger.LogInfo(string.Format("Uživatel {0} se právě zaregistroval", user.UserName), "AccountController.ExternalLoginConfirmation");

                        return this.RedirectToLocal(returnUrl);
                    }
                }

                this.AddErrors(result);
                ViewBag.LoginProvider = info.Login.LoginProvider;
            }

            ViewBag.ReturnUrl = returnUrl;
            model.Title = "Registrace nového uživatele";
            return this.View(model);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.userManager != null)
                {
                    this.userManager.Dispose();
                    this.userManager = null;
                }

                if (this.signInManager != null)
                {
                    this.signInManager.Dispose();
                    this.signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }

            return this.RedirectToHome();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error);
            }
        }

        private void CopyImageFiles(string userName)
        {
            var path = Server.MapPath("~/Images/Profile");
            try
            {
                System.IO.File.Copy(System.IO.Path.Combine(path, "Unknown.jpg"), System.IO.Path.Combine(path, userName + ".jpg"), true);
                System.IO.File.Copy(System.IO.Path.Combine(path, "Unknown_prof.jpg"), System.IO.Path.Combine(path, userName + "_prof.jpg"), true);
            }
            catch (Exception ex)
            {
                logger.LogException(ex, "Account.CopyImageFiles");
            }
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            private const string XsrfKey = "XsrfId";

            public ChallengeResult(string provider, string redirectUri, IAuthenticationManager manager, ILogger logger)
                : this(provider, redirectUri, null, manager, logger)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId, IAuthenticationManager manager, ILogger logger)
            {
                this.LoginProvider = provider;
                this.RedirectUri = redirectUri;
                this.UserId = userId;
                this.Manager = manager;
                this.logger = logger;
            }

            public string LoginProvider { get; set; }

            public string RedirectUri { get; set; }

            public string UserId { get; set; }

            private IAuthenticationManager Manager { get; set; }

            protected ILogger logger { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = this.RedirectUri };
                this.logger.LogDebug($"RedirectUri:{this.RedirectUri}, LoginProvider:{this.LoginProvider}", "ChallengeResult.ExecuteResult");
                if (this.UserId != null)
                {
                    properties.Dictionary[XsrfKey] = this.UserId;
                }

                this.Manager.Challenge(properties, this.LoginProvider);
            }
        }
    }
}