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

        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            try
            {

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