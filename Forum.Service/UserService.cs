using Forum.Common;
using Forum.Models;
using Forum.Models.CommonModels;
using Forum.Models.DataModels;
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Service
{
    public class UserService
    {
        private readonly IOptions<AppSettings> appSettings;
        private readonly HttpContextAccessor httpContextAccessor;
        public UserService(IOptions<AppSettings> _appSettings, HttpContextAccessor _httpContextAccessor)
        {
            appSettings = _appSettings;
            httpContextAccessor = _httpContextAccessor;


        }
        public async Task<GoogleReCaptchaResponse>ReCaptchaVerification(string token)
        {
            GoogleReCaptchaData googleReCaptchaData = new GoogleReCaptchaData
            {
                response = token,
                secret = appSettings.Value.ReCaptchaSecretKey
            };
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={googleReCaptchaData.secret}&response={googleReCaptchaData.response}");
            var captcharesponse = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(response);
            return captcharesponse;
        }

        public async Task SignIn(ApplicationUser user, bool isPersistent = false)
        {
            string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            // Generate Claims from DbEntity
            var claims = AddUserClaims(user);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                // IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.
                // IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.
                // RedirectUri = "~/Account/Index"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await httpContextAccessor.HttpContext.SignInAsync(authenticationScheme, claimsPrincipal, authProperties);
        }

        private List<Claim> AddUserClaims(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("FirstName", user.FirstName));
            claims.Add(new Claim("LastName", user.LastName));
            //claims.Add(new Claim(ClaimTypes.Role, user.AspnetuserRole));
            return claims;
        }

        public List<Claim> GetUserClaims()
        {
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string email = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
            string firstName = httpContextAccessor.HttpContext.User.FindFirst("FirstName")?.Value;
            string lastName = httpContextAccessor.HttpContext.User.FindFirst("LastName")?.Value;
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            claims.Add(new Claim(ClaimTypes.Email, email));
            claims.Add(new Claim("FirstName", firstName));
            claims.Add(new Claim("LastName", lastName));
            //claims.Add(new Claim(ClaimTypes.Role, user.AspnetuserRole));
            return claims;
        }
        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
