using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Models.ViewModels;
using Forum.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Forum.Controllers
{
    public class TopicInformationController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly TopicInformationService topicInformationService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public TopicInformationController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings,
            TopicInformationService _topicInformationService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            topicInformationService = _topicInformationService;
            db = _db;
        }

        [HttpGet]
        [Route("/TopicInformation/Information/{mainTopicId}")]

        public async Task<IActionResult> Information(int mainTopicId)
        {
            ViewBag.MainTopicId = mainTopicId;
            var topicInformations = await topicInformationService.FindTopicInformationByTopicId(mainTopicId);
            return View(topicInformations);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddMainTopicPost()
        {
            return View();
        }

    }
}