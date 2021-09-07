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
    public class ChannelController : Controller
    {
        ForumDbx db;
        private readonly IOptions<AppSettings> appSettings;
        private readonly ChannelService channelService;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public ChannelController(IWebHostEnvironment _webHostEnvironment, IOptions<AppSettings> _appSettings, ChannelService _channelService, ForumDbx _db)
        {
            webHostEnvironment = _webHostEnvironment;
            appSettings = _appSettings;
            channelService = _channelService;
            db = _db;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AddNewChannel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewChannel([FromForm]ChannelViewModel channelViewModel)
        {
            try
            {
                #region saveimage
                var graphics = HttpContext.Request.Form.Files;
                foreach (var Graphics in graphics)
                {
                    if (Graphics != null && Graphics.Length > 0)
                    {
                        var file = Graphics;
                        var uploads = webHostEnvironment.WebRootPath + appSettings.Value.UploadChannelIconPath;
                        //var uploads = Path.Combine(Directory.GetCurrentDirectory(), "~\\Uploads\\");
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(file.FileName);
                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                                string filePath = appSettings.Value.UploadChannelIconPath + fileName;
                                string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                                channelViewModel.ChannelIconURL = fileName;
                            }
                        }
                    }
                }
                #endregion
                var result = await channelService.AddNewChannel(channelViewModel);
                if (result == true)
                {
                    TempData["Success"] = "Channel Successfully Added.";
                    return View();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Oops! some error occured while adding channel.");
                    return View("Error");
                }
                //return View();
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
    }
}