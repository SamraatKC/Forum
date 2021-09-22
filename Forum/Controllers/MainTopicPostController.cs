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
    public class MainTopicPostController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly MainTopicPostService mainTopicPostService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public MainTopicPostController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings,
            MainTopicPostService _mainTopicPostService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            mainTopicPostService = _mainTopicPostService;
            db = _db;
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

        [HttpPost]
        public async Task<IActionResult> AddMainTopicPost([FromForm]MainTopicPostViewModel mainTopicPostViewModel)
        {
            try
            {
                #region saveimage
                //var graphics = HttpContext.Request.Form.Files;
                //foreach (var Graphics in graphics)
                //{
                    if (mainTopicPostViewModel.Graphics != null && mainTopicPostViewModel.Graphics.Length > 0)
                    {
                        var file = mainTopicPostViewModel.Graphics;
                        var uploads = webHostEnvironment.WebRootPath + appSettings.Value.UploadTopicIconPath;
                        //var uploads = Path.Combine(Directory.GetCurrentDirectory(), "~\\Uploads\\");
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                string filePath = appSettings.Value.UploadTopicIconPath + fileName;
                                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                                mainTopicPostViewModel.TopicIcon = fileName;
                            }
                        }
                    }
                //}
                #endregion
                var result = await mainTopicPostService.AddMainTopicPost(mainTopicPostViewModel);
                if (result == true)
                {
                    TempData["Success"] = "Main Topic Post Successfully Added.";
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Oops! some error occured while adding main topic post.");
                    return View("Error");
                }
                //return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<IActionResult> UpdateMainTopicPost(int id)
        {

            var result = await mainTopicPostService.FindMainTopicPostById(id);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMainTopicPost(MainTopicPostViewModel mainTopicPostViewModel)
        {
            var result = await mainTopicPostService.UpdateMainTopicPost(mainTopicPostViewModel);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMainTopicPost()
        {
            try
            {

                
                 var getallmaintopicpost =await  mainTopicPostService.GetAllMainTopicPost();
                if (getallmaintopicpost != null)
                {
                    
                    return View(getallmaintopicpost);
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