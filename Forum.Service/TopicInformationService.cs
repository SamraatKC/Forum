using Forum.Common;
using Forum.Data;
using Forum.Models;
using Forum.Models.DataModels;
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
    public class TopicInformationService
    {
        ForumDbx db;
        IOptions<AppSettings> appSettings;
        public TopicInformationService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            db = new ForumDbx(_appSettings);
        }
        public async Task<bool> AddTopicInformation(TopicInformationViewModel mainTopicPostViewModel)
        {
            TopicInformation topicInformation = new TopicInformation()
            {
                MainTopicsIdFK = mainTopicPostViewModel.MainTopicsIdFK,
                Title = mainTopicPostViewModel.Title,
                Description = mainTopicPostViewModel.Description,
                TopicIcon = mainTopicPostViewModel.TopicIcon,
                CreatedDate = mainTopicPostViewModel.CreatedDate,
                CreatedBy = mainTopicPostViewModel.CreatedBy,
                LastUpdatedDate = mainTopicPostViewModel.LastUpdatedDate,
                LastUpdatedBy = mainTopicPostViewModel.LastUpdatedBy,
                Status = mainTopicPostViewModel.Status,

            };
            await db.TopicInformation.AddAsync(topicInformation);
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<TopicInformationViewModel>> GetAllMainTopicPost()
        {
            var result= await db.TopicInformation.Select(x => (TopicInformationViewModel)x).ToListAsync();
            return result;
        }

        public async Task<List<TopicInformationViewModel>> FindTopicInformationByTopicId(int mainTopicId)
        {
            var res = await db.TopicInformation.Where(x=>x.MainTopicsIdFK == mainTopicId).Select(x=>(TopicInformationViewModel)x).ToListAsync();
            return res;
        }

        public async Task<bool> DeleteMainTopicPostById(int id)
        {
            var mainTopicPost = db.TopicInformation.Where(a => a.TopicInformationId == id).FirstOrDefault();
            db.TopicInformation.Remove(mainTopicPost);
            await db.SaveChangesAsync();
            return true;

        }
    }
}
