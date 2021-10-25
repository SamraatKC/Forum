using Forum.Common;
using Forum.Models;
using Forum.Models.ViewModels;
using Forum.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WanderLust.Common;

namespace Forum.Controllers
{

    public class UserController : Controller
    {
        private readonly AppSettings appSettings;
        private readonly HttpContextAccessor httpContextAccessor;
        private readonly UserService userService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly EmailHelper emailHelper;
        private readonly HostgatorEmailHelper hostgatorEmailHelper;

        #region Constryctor
        public UserController(HttpContextAccessor _httpContextAccessor, IOptions<AppSettings> _appSettings, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager
            , UserService _userService, EmailHelper _emailHelper, HostgatorEmailHelper _hostgatorEmailHelper,
             RoleManager<IdentityRole> _roleManager)
        {
            hostgatorEmailHelper = _hostgatorEmailHelper;
            emailHelper = _emailHelper;
            appSettings = _appSettings.Value;
            userManager = _userManager;
            signInManager = _signInManager;
            userService = _userService;
            roleManager = _roleManager;
            httpContextAccessor = _httpContextAccessor;
        }
        #endregion


        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsUserNameTaken(string username)
        {
            var isUserNameTaken = await userManager.FindByNameAsync(username);
            if (isUserNameTaken == null)
            {

                return Json(true);
            }
            else
            {
                return Json($"User name already taken");
            }

        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailTaken(string email)
        {
            var isEmailTaken = await userManager.FindByEmailAsync(email);
            if (isEmailTaken == null)
            {

                return Json(true);
            }
            else
            {
                return Json($"Email already taken");
            }

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            try
            {
                string normaluserRole = Enum.GetName(typeof(Enums.RoleNames), 1);
                string defaultRoleName = string.IsNullOrEmpty(userViewModel.RoleName) ? normaluserRole : userViewModel.RoleName;
                string nomalUserRole = Enum.GetName(typeof(Enums.RoleNames), 1);

                #region if role is not found or duplicate user found then return error message
                var asp_role = await roleManager.FindByNameAsync(defaultRoleName);
                //var asp_user = await userManager.FindByEmailAsync(userViewModel.Email);
                if (asp_role == null)
                {
                    //ModelState.AddModelError(string.Empty, "Role not foumd");
                    await roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = normaluserRole
                    });
                    asp_role = await roleManager.FindByNameAsync(defaultRoleName);

                }
                //if (asp_user != null)
                //    ModelState.AddModelError(string.Empty, "Duplicate role found");

                #endregion

                #region construct ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = userViewModel.Email,
                    Email = userViewModel.Email,
                    PasswordHash = userViewModel.Password,
                    FirstName = userViewModel.FirstName,
                    LastName = userViewModel.LastName,
                };
                #endregion
                #region CreateUser
                var result = await userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    var addusertorole = await userManager.AddToRoleAsync(user, asp_role.Name);                                   
                    #region Send Account Activation Email
                    string code = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                    code = System.Web.HttpUtility.UrlEncode(code);
                    string url = Url.Action(nameof(ConfirmEmail), "User", new { userid = user.Id, code = code }, Request.Scheme, Request.Host.ToString());

                    string verificationLink = string.Format("{0}{1}{2}{3}{4}", appSettings.JwtIssuer, appSettings.VerificationLink, user.Id, "&code=", code);
                    string htmlEmailBody = emailHelper.GetEmailBody(appSettings.EmailTemplate_AccountVerification);
                    htmlEmailBody = htmlEmailBody.Replace("{FirstName}", userViewModel.FirstName);
                    htmlEmailBody = htmlEmailBody.Replace("{Email}", userViewModel.Email);
                    htmlEmailBody = htmlEmailBody.Replace("{Password}", userViewModel.Password);
                    htmlEmailBody = htmlEmailBody.Replace("{ActivationLink}", url);
                    emailHelper.SendEmail("Account Activation - Forum", userViewModel.Email, htmlEmailBody);
                    //hostgatorEmailHelper.SendEmail();
                    #endregion
                    TempData["Message"] = "An account verification link has been sent in your associated mail.";
                    return View("Login");
                }
                else
                {
                    TempData["Message"] = "Oops! some error occured while creating your account, please contact system provider.";
                    return View("Login");
                }
                #endregion


            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please fill up the fields.");
                return View();
            }
            //return View();
        }
        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userid, string code)
        {
            // ModelState.AddModelError(string.Empty, "Oops! some error occured while activating your account. Please contact system provider."); 
            if (userid == null || code == null)
            {
                ModelState.AddModelError(string.Empty, "Oops! some error occured while activating your account. Please contact system provider.");
                return View("Register");
            }
            code = System.Web.HttpUtility.UrlDecode(code);
            IdentityResult result;
            try
            {
                var user = await userManager.FindByIdAsync(userid);
                if (user == null) return Redirect(appSettings.JwtAudience);
                result = await userManager.ConfirmEmailAsync(user, code);
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                return View("Register");
            }
            if (result.Succeeded)
            {

                return View("AccountConfirmed");

            }
            if (!result.Succeeded)
            {
                return View("Register");
            }
            return View("Register");

        }

        [HttpGet]
        public async Task<IActionResult> Login(string provider, string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                //var GoogleReCaptcha = userService.ReCaptchaVerification(loginViewModel.Token);
                //if (!GoogleReCaptcha.Result.success && GoogleReCaptcha.Result.score <= 0.5)
                //{
                //    ModelState.AddModelError(string.Empty, "You are not human.");
                //    return View();
                //}
                var user = await userManager.FindByEmailAsync(loginViewModel.Email);
                if (user != null)
                {
                    await userService.SignIn(user, false);
                    var userRole = await userManager.GetRolesAsync(user);
                    if (userRole != null)
                    {
                        var role = userRole.FirstOrDefault();
                        switch (role.ToLower())
                        {
                            case "admin":
                                return RedirectToAction("AdminDashboard", "Admin");
                            case "normaluser":
                                return RedirectToAction("RegularUserDashboard", "RegularUser");
                        }

                    }
                    else
                    {

                        TempData["Message"] = "Role has not been assigned to you, please contact system provider about your issue";
                        return View("Login");
                    }
                    ModelState.AddModelError(string.Empty, "User name or password is invalid");
                    return View();

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "User name or password is invalid");
                return View();
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    string url = Url.Action(nameof(ResetPassword), "User", new { userid = user.Id }, Request.Scheme);
                    //string url = string.Format("{0}{1}/{2}", appSettings.AllowedOrigin, appSettings.PasswordReset,user.Id );
                    if (url != null)
                    {
                        #region Send Forgot Password Email
                        string htmlEmailBody = emailHelper.GetEmailBody(appSettings.EmailTemplate_ForgotPassword);
                        htmlEmailBody = htmlEmailBody.Replace("{FirstName}", user.FirstName);
                        htmlEmailBody = htmlEmailBody.Replace("{ResetPasswordLink}", url);
                        emailHelper.SendEmail("Password Reset - Forum", user.Email, htmlEmailBody);
                        TempData["Success"] = "Password reset link has been sent in your associated mail.";
                        return View();
                        #endregion
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Not Found");
                    return View();
                }



            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please enter your email address.");
                return View();
            }


        }
        [HttpGet]
        public IActionResult ResetPassword(string userid)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel()
            {
                Id = userid,
                NewPassword = null,
            };
            //ViewBag.UserId = userid;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            try
            {
                var userid = resetPasswordViewModel.Id;
                var newpassword = resetPasswordViewModel.NewPassword;
                var checkUser = await userManager.FindByIdAsync(userid);
                if (checkUser != null)
                {
                    string token = await userManager.GeneratePasswordResetTokenAsync(checkUser);
                    var changePassword = await userManager.ResetPasswordAsync(checkUser, token, newpassword);

                    if (changePassword != null)
                    {
                        TempData["Success"] = "Password successfully reset.";
                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Something went wrong");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User Not Found.");
                    return View();

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Internal Server Error");
                return View();
            }

        }
        private object GenerateJwtToken(string email, ApplicationUser user)//, IList<string> roles
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("FullName" , string.Format("{0} {1}", user.FirstName, user.LastName) ),
                //new Claim(ClaimTypes.MobilePhone, user.PhoneNumber?? "")
                //new Claim("Roles", string.Join(",",roles))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JwtKey));
            var key_phrase = appSettings.JwtKey;
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(appSettings.JwtExpireDays));
            var token = new JwtSecurityToken(
                appSettings.JwtIssuer,
                appSettings.JwtAudience,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize]
        public IActionResult SecretView()
        {

            return View();
        }

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> CreateRole(RoleViewModel model)
        {
            try
            {
                #region if role is not found or duplicate user found then return error message
                var asp_role = await roleManager.FindByNameAsync(model.Name);

                if (asp_role != null)
                {
                    ModelState.AddModelError(string.Empty, "Role Already Exist");
                    return View();
                }

                var res = await roleManager.CreateAsync(new IdentityRole(model.Name));
                if (res.Succeeded)
                {
                    TempData["Success"] = "Role created";
                    return View();
                }
                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }


        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return LocalRedirect("/");

        }


        #region ExternalLogin

       public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "User", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogin = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error Loading External Login Information");
                return View("Login", loginViewModel);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error Loading External Login Information");
                return View("Login", loginViewModel);
            }
            var signInResult = await signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (signInResult.Succeeded)
            {
                return View("SecretView");
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return View("SecretView");
                }
                return View("ErrorViewModel");
            }

        }
        #endregion
    }
}