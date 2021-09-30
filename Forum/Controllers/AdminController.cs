using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Forum.Controllers
{
    public class AdminController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly AdminService adminService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings, AdminService _adminService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            adminService = _adminService;
            db = _db;
        }

        public IActionResult TestView()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            try
            {
                List<SelectListItem> moderators = new List<SelectListItem>()
            {
                new SelectListItem { Value = "1", Text = "Moderator1" },
                new SelectListItem { Value = "2", Text = "Moderator2" },
                new SelectListItem { Value = "3", Text = "Moderator3" },
               
            };

                //assigning SelectListItem to view Bag
                ViewBag.moderators = moderators;

                var getallmaintopic = await adminService.GetAllMainTopic();
                if (getallmaintopic != null)
                {
                    return View(getallmaintopic);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Oops! some error occured while loading main topics.");
                    return View();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}