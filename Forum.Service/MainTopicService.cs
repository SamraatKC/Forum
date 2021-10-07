﻿using Forum.Common;
using Forum.Data;
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
                Title = mainTopicViewModel.Title,
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
        public async Task<bool> UpdateMainTopic(MainTopicViewModel mainTopicViewModel)
        {
            MainTopic mainTopic = await db.MainTopics.FindAsync(mainTopicViewModel.MainTopicId);
            if (mainTopic != null)
            {
                mainTopic.ThemeIdFK = mainTopicViewModel.ThemeIdFK;
                mainTopic.ParentIdFK = mainTopicViewModel.ParentIdFK;
                mainTopic.ReferenceLink = mainTopicViewModel.ReferenceLink;
                mainTopic.Title = mainTopicViewModel.Title;
                mainTopic.Description = mainTopicViewModel.Description;
                mainTopic.TopicIcon = mainTopicViewModel.TopicIcon;
                mainTopic.DisplayOrder = mainTopicViewModel.DisplayOrder;
                mainTopic.CreatedDate = mainTopicViewModel.CreatedDate;
                mainTopic.CreatedBy = mainTopicViewModel.CreatedBy;
                mainTopic.LastUpdatedDate = mainTopicViewModel.LastUpdatedDate;
                mainTopic.LastUpdatedBy = mainTopicViewModel.LastUpdatedBy;
                mainTopic.Status = mainTopicViewModel.Status;
                await db.SaveChangesAsync();
                return true;
            };
            return false;
        }

        public List<MainTopicViewModel> GetAllMainTopic()
        {
            var result = db.MainTopics
                .Select(x => new MainTopicViewModel
                {
                    MainTopicId = x.MainTopicId,
                    ThemeIdFK = x.ThemeIdFK,
                    ParentIdFK = x.ParentIdFK,
                    ReferenceLink = x.ReferenceLink,
                    Title = x.Title,
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
    }
}
