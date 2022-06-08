using _3F.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;
using System;
using System.Web.Mvc;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;

namespace _3F.Web
{
    public partial class Startup
    {
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }
        
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            //// Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(DependencyResolver.Current.GetService<ApplicationDbContext>);
            app.CreatePerOwinContext(DependencyResolver.Current.GetService<ApplicationUserManager>);
            app.CreatePerOwinContext(DependencyResolver.Current.GetService<ApplicationSignInManager>);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser, int>(
                        validateInterval: TimeSpan.FromDays(7),
                        regenerateIdentityCallback: (manager, user) => user.GenerateUserIdentityAsync(manager),
                        getUserIdCallback: id => (id.GetUserId<int>()))
                },
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            var fbOptions = new FacebookAuthenticationOptions()
            {
                AppId = "1473953056232617",
                AppSecret = "0170e1dcf35061e811f64ec499efd91d",
            };

            fbOptions.Scope.Add("public_profile");
            fbOptions.Scope.Add("email");
            app.UseFacebookAuthentication(fbOptions);

            var gOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "580064212784-9glg7d611663318lk50ipa72ab1vnj8l.apps.googleusercontent.com",
                ClientSecret = "dQPh3yn1njE-WmUrTI3amjUE"
            };

            app.UseGoogleAuthentication(gOptions);

        }
    }
}