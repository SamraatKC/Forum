using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
using static Forum.Common.Enums;

namespace Forum.Controllers
{
    public class TopicInformationController : BaseController
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly TopicInformationService topicInformationService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserService userService;

        public TopicInformationController(UserService _userService,IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings,
            TopicInformationService _topicInformationService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            topicInformationService = _topicInformationService;
            db = _db;
            userService = _userService;
        }

        [HttpGet]
        [Route("/TopicInformation/Information/{mainTopicId}/{parentIdFk}")]

        public async Task<IActionResult> Information(int mainTopicId, int parentIdFk)
        {
            ViewBag.MainTopicId = mainTopicId;
            ViewBag.ParentIdFk = parentIdFk;
            var topicInformations = await topicInformationService.FindTopicInformationByTopicId(mainTopicId);

            return View(topicInformations);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTopicInformation(TopicInformationViewModel topicInformationViewModel,int? id=null)
        {
            try
            {
                topicInformationViewModel.Status = StatusEnum.New.ToString();
                List<Claim> userClaims = userService.GetUserClaims();

                //var user = await userManager.FindByEmailAsync(currentUserEmail);

                topicInformationViewModel.CreatedBy = userClaims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
                topicInformationViewModel.CreatedDate = System.DateTime.Now;

                #region saveimage
                //var graphics = HttpContext.Request.Form.Files;
                //foreach (var Graphics in graphics)
                //{
                if (topicInformationViewModel.Graphics != null && topicInformationViewModel.Graphics.Length > 0)
                {
                    var file = topicInformationViewModel.Graphics;
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
                            topicInformationViewModel.TopicIcon = fileName;
                        }
                    }
                }
                // }
                #endregion

                if (topicInformationViewModel.TopicInformationId == 0)
                {
                    var result = await topicInformationService.AddTopicInformation(topicInformationViewModel);
                    if (result == true)
                    {
                        id = topicInformationViewModel.MainTopicsIdFK;
                        TempData["Success"] = "Topic Information Successfully Added.";
                        
                        return RedirectToAction("~/TopicInformation/Information/"+id );
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Oops! some error occured while adding  topic informaion.");
                        return View("Error");
                    }
                }
                else
                {
                    var data = await topicInformationService.UpdateTopicInformation(topicInformationViewModel);
                    if (data != null)
                    {
                        TempData["Success"] = " Topic Information Successfully Updated.";
                        return Redirect("~/Admin/AdminDashboard");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Oops! some error occured while updating  topic information.");
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


    }
}