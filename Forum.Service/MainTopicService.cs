using Forum.Common;
using Forum.Data;
using Forum.Models.DataModels;
using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Forum.Service
{
    public class MainTopicService
    {
        ForumDbx db;
        IOptions<AppSettings> appSettings;
       
        public MainTopicService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            db = new ForumDbx(_appSettings);
          
        }
        public async Task<bool> AddMainTopic(MainTopicViewModel mainTopicViewModel)
        {
            MainTopic mainTopic = new MainTopic()
            {
                ThemeIdFK = mainTopicViewModel.ThemeIdFK,
                ParentIdFK = mainTopicViewModel.ParentIdFK,
                ReferenceLink = mainTopicViewModel.ReferenceLink,
                Topic = mainTopicViewModel.Topic,
                Description = mainTopicViewModel.Description,
                TopicIcon = mainTopicViewModel.TopicIcon,
                DisplayOrder = mainTopicViewModel.DisplayOrder,
                CreatedDate = mainTopicViewModel.CreatedDate,
                CreatedBy = mainTopicViewModel.CreatedBy,
                LastUpdatedDate = mainTopicViewModel.LastUpdatedDate,
                LastUpdatedBy = mainTopicViewModel.LastUpdatedBy,
                Status = mainTopicViewModel.Status,

            };
            await db.MainTopics.AddAsync(mainTopic);
            await db.SaveChangesAsync();
            return true;
        }
        //public async Task<bool> UpdateMainTopic(MainTopicViewModel mainTopicViewModel)
        //{
        //    MainTopic mainTopic = await db.MainTopics.FindAsync(mainTopicViewModel.MainTopicId);
        //    if (mainTopic != null)
        //    {
        //        mainTopic.ThemeIdFK = mainTopicViewModel.ThemeIdFK;
        //        mainTopic.ParentIdFK = mainTopicViewModel.ParentIdFK;
        //        mainTopic.ReferenceLink = mainTopicViewModel.ReferenceLink;
        //        mainTopic.Title = mainTopicViewModel.Title;
        //        mainTopic.Description = mainTopicViewModel.Description;
        //        mainTopic.TopicIcon = mainTopicViewModel.TopicIcon;
        //        mainTopic.DisplayOrder = mainTopicViewModel.DisplayOrder;
        //        mainTopic.CreatedDate = mainTopicViewModel.CreatedDate;
        //        mainTopic.CreatedBy = mainTopicViewModel.CreatedBy;
        //        mainTopic.LastUpdatedDate = mainTopicViewModel.LastUpdatedDate;
        //        mainTopic.LastUpdatedBy = mainTopicViewModel.LastUpdatedBy;
        //        mainTopic.Status = mainTopicViewModel.Status;
        //        await db.SaveChangesAsync();
        //        return true;
        //    };
        //    return false;
        //}

        public List<MainTopicViewModel> GetAllMainTopic()
        {
            var result = db.MainTopics
                .Select(x => new MainTopicViewModel
                {
                    MainTopicId = x.MainTopicId,
                    ThemeIdFK = x.ThemeIdFK,
                    ParentIdFK = x.ParentIdFK,
                    ReferenceLink = x.ReferenceLink,
                    Topic = x.Topic,
                    Description = x.Description,
                    TopicIcon = x.TopicIcon,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    LastUpdatedDate = x.LastUpdatedDate,
                    LastUpdatedBy = x.LastUpdatedBy,
                    Status = x.Status,
                    Moderator = x.Moderator,
                    HasItems = db.MainTopics.Count(y=>y.ParentIdFK == x.MainTopicId) > 0,
                }).ToList();

            return result;
        }

        public async Task<List<MainTopicViewModel>> GetParentAndSubTopic(int topicId)
        {
            var result = await db.MainTopics.Where(x=>x.MainTopicId == topicId || x.ParentIdFK == topicId)
                .Select(x => new MainTopicViewModel
                {
                    MainTopicId = x.MainTopicId,
                    ThemeIdFK = x.ThemeIdFK,
                    ParentIdFK = x.ParentIdFK,
                    ReferenceLink = x.ReferenceLink,
                    Topic = x.Topic,
                    Description = x.Description,
                    TopicIcon = x.TopicIcon,
                    DisplayOrder = x.DisplayOrder,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    LastUpdatedDate = x.LastUpdatedDate,
                    LastUpdatedBy = x.LastUpdatedBy,
                    Status = x.Status,
                    Moderator = x.Moderator,
                    HasItems = db.MainTopics.Count(y => y.ParentIdFK == x.MainTopicId) > 0,
                }).ToListAsync();

            return result;
        }

        //public async Task<List<MainTopic>> GetAllMainTopic()
        //{
        //    var reult= await db.MainTopics.ToListAsync();
        //    return reult;
        //}
        //public async Task<IEnumerable<MainTopicViewModel>> GetAllMainTopic()
        //{
        //    List<MainTopicViewModel> mainTopicViewModel = await db.MainTopics
        //        .OrderByDescending(x => x.MainTopicId)
        //        .Select(x => new MainTopicViewModel { })
        //        .ToListAsync();
        //    return mainTopicViewModel;
        //}

        public async Task<MainTopicViewModel> FindMainTopicById(int id)
        {
            var res = await db.MainTopics.FindAsync(id);
            return res;
        }

        public async Task<bool> DeleteMainTopictById(int id)
        {
            var mainTopicId = db.MainTopics.OrderBy(e => e.MainTopicId).Where(a => a.MainTopicId == id).FirstOrDefault();
            db.MainTopics.Remove(mainTopicId);
            await db.SaveChangesAsync();
            return true;

        }

        public async Task<bool> CheckMainTopicParentDependencies(int id)
        {
            var result =  db.MainTopics.Any(x => x.ParentIdFK == id);
            if(result)
            {
                return true;
            }
            return false;
        }
        public async Task<MainTopicViewModel> UpdateMainTopic(MainTopicViewModel mainTopicViewModel)
        {
            if (mainTopicViewModel.MainTopicId > 0)
            {
                var result = await db.MainTopics.FirstOrDefaultAsync(e => e.MainTopicId == mainTopicViewModel.MainTopicId);
                result.Topic = mainTopicViewModel.Topic;
                result.TopicIcon = mainTopicViewModel.TopicIcon;
                result.Description = mainTopicViewModel.Description;
                result.DisplayOrder = mainTopicViewModel.DisplayOrder;
                result.ParentIdFK = mainTopicViewModel.ParentIdFK;

                if (await db.SaveChangesAsync() > 0)
                    return mainTopicViewModel;
                else
                {
                    //-1 meaning not able to update the record.
                    mainTopicViewModel.MainTopicId = -1;
                    return mainTopicViewModel;
                }
            }
            else
            {
                MainTopic mainTopic = new MainTopic();
                mainTopic = (MainTopic)mainTopicViewModel;
                db.MainTopics.Add(mainTopic);
                await db.SaveChangesAsync();
                return (MainTopicViewModel)mainTopic;
            }
        }
    }
}
