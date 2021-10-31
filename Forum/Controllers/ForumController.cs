using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Models.DataModels;
using Forum.Models.ViewModels;
using Forum.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using static Forum.Common.Enums;

namespace Forum.Controllers
{
    public class ForumController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly MainTopicService mainTopicService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserService userService;
        public ForumController(IHttpContextAccessor _httpContextAccessor, IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings, MainTopicService _mainTopicService, ForumDbx _db, UserService _userService)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            mainTopicService = _mainTopicService;
            db = _db;
            httpContextAccessor = _httpContextAccessor;
            userService = _userService;
            
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]

        public IActionResult AddMainTopic()
        {
            if (ModelState.IsValid)
            {
                //fettch all the topics from table MainTopics with Text as title and value as id,
                var parentTopics = mainTopicService.GetAllMainTopic()
                    .Select(x => new SelectListItem { Text = x.Title, Value = x.MainTopicId.ToString() }).ToList();
                //.ToList(x => x.MainTopicId, y => y.Title);
                ViewBag.ParentTopic = parentTopics;
                return View();

            }
            return RedirectToAction("AdminDashboard", "Admin");
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMainTopic([FromForm] MainTopicViewModel mainTopicViewModel)
        {
            try
            {
                mainTopicViewModel.Status = StatusEnum.New.ToString();
                List<Claim> userClaims = userService.GetUserClaims();

                //var user = await userManager.FindByEmailAsync(currentUserEmail);

                mainTopicViewModel.CreatedBy = userClaims.Where( x=> x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
                mainTopicViewModel.CreatedDate = System.DateTime.Now;

                #region saveimage
                //var graphics = HttpContext.Request.Form.Files;
                //foreach (var Graphics in graphics)
                //{
                if (mainTopicViewModel.Graphics != null && mainTopicViewModel.Graphics.Length > 0)
                {
                    var file = mainTopicViewModel.Graphics;
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
                            mainTopicViewModel.TopicIcon = fileName;
                        }
                    }
                }
                // }
                #endregion

                if(mainTopicViewModel.MainTopicId==0)
                {
                    var result = await mainTopicService.AddMainTopic(mainTopicViewModel);
                    if (result == true)
                    {
                        TempData["Success"] = "Main Topic Successfully Added.";
                        return Redirect("~/Admin/AdminDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Oops! some error occured while adding main topic.");
                        return View("Error");
                    }
                }
                else
                {
                    var data = await mainTopicService.UpdateMainTopic(mainTopicViewModel);
                    if(data!=null)
                    {
                        TempData["Success"] = "Main Topic Successfully Updated.";
                        return Redirect("~/Admin/AdminDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Oops! some error occured while updating main topic.");
                        return View("Error");
                    }
                }
                //return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public async Task<IActionResult> UpdateMainTopic(int id)
        {

            var result = await mainTopicService.FindMainTopicById(id);
            return View(result);
        }






























































        [HttpPost]
        public async Task<IActionResult> UpdateMainTopic(MainTopicViewModel mainTopicViewModel)
        {
            var result = await mainTopicService.UpdateMainTopic(mainTopicViewModel);
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllMainTopic()
        {
            try
            {

                var getallmaintopic = mainTopicService.GetAllMainTopic();
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

        [HttpGet]
        [Route("/Forum/SubTopic/{mainTopicId}")]
        public async Task<IActionResult> SubTopic(int mainTopicId)
        {
            ViewBag.MainTopicId = mainTopicId;
            var topicAndItsSubTopic = await mainTopicService.GetParentAndSubTopic(mainTopicId);
            return View(topicAndItsSubTopic);
        }

        [HttpGet]
        public async Task<JsonResult> GetParentAndSubTopic(int topicId, DataSourceLoadOptions loadOptions)
        {
            try
            {
                var topicAndItsSubTopic = await mainTopicService.GetParentAndSubTopic(topicId);
                if (topicAndItsSubTopic != null)
                {
                    //return new JsonResult(topicAndItsSubTopic);
                    return new JsonResult(DataSourceLoader.Load(topicAndItsSubTopic, loadOptions));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Oops! some error occured while loading main topics.");
                    return new JsonResult(DataSourceLoader.Load(new List<MainTopicViewModel>(), loadOptions));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}