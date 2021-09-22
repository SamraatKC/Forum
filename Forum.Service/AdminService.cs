using Forum.Common;
using Forum.Data;
using Forum.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Service
{
    public class AdminService
    {
        ForumDbx db;
        IOptions<AppSettings> appSettings;
        public AdminService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            db = new ForumDbx(_appSettings);
        }

        public async Task<List<MainTopicViewModel>> GetAllMainTopic()
        {
            var result = await db.MainTopics.Select(x => (MainTopicViewModel)x).ToListAsync();
            return result;
        }
    }
}
