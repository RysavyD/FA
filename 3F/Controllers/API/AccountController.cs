using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web.Http;
using _3F.Model;
using _3F.Model.Model;
using _3F.Web.Extensions;
using _3F.Web.Models;
using _3F.Web.Utils;

namespace _3F.Web.Controllers.API
{
    public class AccountController : ApiController
    {
        private IRepository _repository;

        public AccountController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public HttpResponseMessage Login(string userName, string password)
        {
            var resp = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent("Nepřihlášen")
            };

            var user = _repository.One<AspNetUsers>(u => u.UserName.ToLower() == userName.ToLower());

            if (user != null)
            {
                if (user.LoginType == LoginTypeEnum.OldSystemConfirmed)
                {
                    var oldUser = user.OldPassword.First();
                    if (Utilities.Validate(oldUser, password))
                    {
                        AddCookie(resp, user.UserName, user.AspNetRoles.Select(r => r.Name).ToArray());
                    }
                }
                else if (user.LoginType == LoginTypeEnum.Confirmed)
                {
                    if (VerifyHashedPassword(user.PasswordHash, password))
                    {
                        AddCookie(resp, user.UserName, user.AspNetRoles.Select(r => r.Name).ToArray());
                    }
                }
            }

            return resp;
        }

        private void AddCookie(HttpResponseMessage message, string userName, string[] roles)
        {
            var value = userName + "|" + string.Join(",", roles);

            var cookie = new CookieHeaderValue(".ApiLoginCookie", Utilities.Crypt(value))
            {
                Expires = DateTimeOffset.Now.AddYears(1),
                Domain = Request.RequestUri.Host,
                Path = "/"
            };

            message.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            message.Content = new StringContent("Přihlášen");
        }

        private bool VerifyHashedPassword(string hashedPassword, string password)
        {
            if (hashedPassword == null)
            {
                return false;
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            byte[] array = Convert.FromBase64String(hashedPassword);
            if (array.Length != 49 || array[0] != 0)
            {
                return false;
            }
            byte[] array2 = new byte[16];
            Buffer.BlockCopy(array, 1, array2, 0, 16);
            byte[] array3 = new byte[32];
            Buffer.BlockCopy(array, 17, array3, 0, 32);
            byte[] bytes;
            using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, array2, 1000))
            {
                bytes = rfc2898DeriveBytes.GetBytes(32);
            }
            return ByteArraysEqual(array3, bytes);
        }

        private bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            bool flag = true;
            for (int i = 0; i < a.Length; i++)
            {
                flag &= (a[i] == b[i]);
            }
            return flag;
        }

        [HttpGet, ApiAuthorize]
        public IHttpActionResult Check()
        {
            return Json(new User()
            {
                name = User.Identity.Name,
            });
        }
    }
}
