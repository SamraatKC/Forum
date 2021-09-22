﻿using Forum.Common;
using Forum.Models;
using Forum.Models.ViewModels;
using Forum.Service;
using Microsoft.AspNetCore.Authorization;
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
        #region Constryctor
        private readonly AppSettings appSettings;
        private readonly UserService userService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly EmailHelper emailHelper;
        private readonly HostgatorEmailHelper hostgatorEmailHelper;
        public UserController(IOptions<AppSettings> _appSettings, UserManager<ApplicationUser> _userManager, SignInManager<ApplicationUser> _signInManager
            , UserService _userService, EmailHelper _emailHelper,HostgatorEmailHelper _hostgatorEmailHelper,
             RoleManager<IdentityRole> _roleManager)
        {
            hostgatorEmailHelper = _hostgatorEmailHelper;
            emailHelper = _emailHelper;
            appSettings = _appSettings.Value;
            userManager = _userManager;
            signInManager = _signInManager;
            userService = _userService;
            roleManager = _roleManager;
        }
        #endregion
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

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


        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel userViewModel)
        {
            try
            {
                string normaluserRole = Enum.GetName(typeof(Enums.RoleNames),1 );
                string defaultRoleName = string.IsNullOrEmpty(userViewModel.RoleName) ? normaluserRole : userViewModel.RoleName;
                int userCodeIndex = userViewModel.FirstName.ToUpper().IndexOf(appSettings.NormalUserCode.ToUpper());
                if (userCodeIndex > 0)
                {
                    string adminRole = Enum.GetName(typeof(Enums.RoleNames), 0);
                    defaultRoleName = string.IsNullOrEmpty(userViewModel.RoleName) ? adminRole : userViewModel.RoleName;
                    userViewModel.FirstName = userViewModel.FirstName.Substring(0, userCodeIndex);
                }
                #region if role is not found or duplicate user found then return error message
                var asp_role = await roleManager.FindByNameAsync(defaultRoleName);
                //var asp_user = await userManager.FindByEmailAsync(userViewModel.Email);
                if (asp_role == null)
                {
                    ModelState.AddModelError(string.Empty, "Role not foumd");

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
                var addusertorole = await userManager.AddToRoleAsync(user, asp_role.Name);
                if (result.Succeeded)
                {
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
                    TempData["Success"] = "An account verification link has been sent in your associated mail.";
                    return View("Register");
                }
                else
                {
                    return View("Register");
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
        public IActionResult Login()
        {
            return View();
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
                    var result = await signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        var appUser = userManager.Users.SingleOrDefault(r => r.Email == loginViewModel.Email);
                        var token = string.Format("{0}", GenerateJwtToken(loginViewModel.Email, appUser));
                        var resp = new { Token = token };
                        if (await userManager.IsInRoleAsync(appUser, "Admin"))
                        {
                            return View("AdminView");
                        }
                        return View("UserView");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Please confirm your account to login.");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please enter your credentials.");
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
                if(user!=null)
                {
                    string url = Url.Action(nameof(ResetPassword), "User", new { userid = user.Id},Request.Scheme);
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


    }
}