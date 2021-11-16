using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Forum.Controllers
{
    public class AdminController : BaseController
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly AdminService adminService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly MainTopicService mainTopicService;

        public AdminController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings, AdminService _adminService, ForumDbx _db, MainTopicService _mainTopicService)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            adminService = _adminService;
            db = _db;
            mainTopicService = _mainTopicService;
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
                ViewBag.moderators = moderators;

                var parentTopics = mainTopicService.GetAllMainTopic()
               .Select(x => new SelectListItem { Text = x.Topic, Value = x.MainTopicId.ToString() }).ToList();
                //.ToList(x => x.MainTopicId, y => y.Title);
                ViewBag.ParentTopic = parentTopics;
                var getallmaintopic = await adminService.GetAllMainTopic();
                if (getallmaintopic != null)
                {
                    return View();
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
        [HttpGet]
        public JsonResult GetMainTopics(DataSourceLoadOptions loadOptions)
        {
            var data = mainTopicService.GetAllMainTopic();
            return new JsonResult(DataSourceLoader.Load(data, loadOptions));
        }

        [HttpGet]
        public JsonResult GetMainTopicById(int id)
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            var mainTopicById = mainTopicService.FindMainTopicById(id);
            var res = JsonSerializer.Serialize(mainTopicById, options);
            return Json(res);
        }
        [HttpPost]
        public async Task<ActionResult>DeleteMainTopicById(int id)
        {
            try
            {
                JsonSerializerOptions options = new()
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var checkMainTopicParentDependencies = await mainTopicService.CheckMainTopicParentDependencies(id);
                if(checkMainTopicParentDependencies==true)
                {

                    return Json(null);
                }
                else
                {
                    
                    var deleteMainTopicById = await mainTopicService.DeleteMainTopictById(id);
                    if (deleteMainTopicById == true)
                    {
                        var res = JsonSerializer.Serialize(deleteMainTopicById, options);
                        return Json(res);
                    }
                    else
                    {
                        return Json(null);
                    }
                }
              
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}