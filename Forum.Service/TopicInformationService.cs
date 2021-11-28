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

        public async Task<TopicInformationViewModel> UpdateTopicInformation(TopicInformationViewModel mainTopicPostViewModel)
        {
            if (mainTopicPostViewModel.TopicInformationId > 0)
            {
                var result = await db.TopicInformation.FirstOrDefaultAsync(e => e.TopicInformationId == mainTopicPostViewModel.TopicInformationId);
                result.Title = mainTopicPostViewModel.Title;
                result.Description = mainTopicPostViewModel.Description;
                result.TopicIcon = mainTopicPostViewModel.TopicIcon;
                result.CreatedDate = mainTopicPostViewModel.CreatedDate;
                result.CreatedBy = mainTopicPostViewModel.CreatedBy;
                result.LastUpdatedDate = mainTopicPostViewModel.LastUpdatedDate;
                result.LastUpdatedBy = mainTopicPostViewModel.LastUpdatedBy;
                result.Status = mainTopicPostViewModel.Status;
               
                if (await db.SaveChangesAsync() > 0)
                    return mainTopicPostViewModel;
                else
                {
                    //-1 meaning not able to update the record.
                    mainTopicPostViewModel.TopicInformationId = -1;
                    return mainTopicPostViewModel;
                }
            }
            else
            {
                TopicInformation topicInformation = new TopicInformation();
                topicInformation = (TopicInformation)mainTopicPostViewModel;
                db.TopicInformation.Add(topicInformation);
                await db.SaveChangesAsync();
                return (TopicInformationViewModel)topicInformation;
            }
        }

        public async Task<List<TopicInformationViewModel>> GetAllTopicInformation()
        {
            var result= await db.TopicInformation.Select(x => (TopicInformationViewModel)x).ToListAsync();
            return result;
        }

        public async Task<List<TopicInformationViewModel>> GetTopicAndTopicInformation(int topicId)
        {

            var result = await db.TopicInformation.Where(x => x.MainTopicsIdFK == topicId)
                .Select(x => new TopicInformationViewModel
                {
                    //TopicInformation = x.TopicInformation,
                    TopicInformationId = x.TopicInformationId,
                    MainTopicsIdFK = x.MainTopicsIdFK,
                   Title = x.Title,
                    Description = x.Description,
                    TopicIcon = x.TopicIcon,
                    
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    LastUpdatedDate = x.LastUpdatedDate,
                    LastUpdatedBy = x.LastUpdatedBy,
                    Status = x.Status,
                 }).ToListAsync();

            return result;
        }
        public async Task<List<TopicInformationViewModel>> FindTopicInformationByTopicId(int mainTopicId)
        {
            var res = await db.TopicInformation.Where(x=>x.MainTopicsIdFK == mainTopicId).Select(x=>(TopicInformationViewModel)x).ToListAsync();
            return res;
        }

        public async Task<bool> DeleteTopicInformationById(int id)
        {
            var mainTopicPost = db.TopicInformation.Where(a => a.TopicInformationId == id).FirstOrDefault();
            db.TopicInformation.Remove(mainTopicPost);
            await db.SaveChangesAsync();
            return true;

        }
    }
}
