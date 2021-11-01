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
    public class RegularUserController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly RegularUserService regularUserService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RegularUserController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings,
            RegularUserService _regularUserService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            regularUserService = _regularUserService;
            db = _db;
        }
    }
}