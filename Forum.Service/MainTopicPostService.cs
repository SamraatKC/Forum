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
    public class MainTopicPostService
    {
        ForumDbx db;
        IOptions<AppSettings> appSettings;
        public MainTopicPostService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            db = new ForumDbx(_appSettings);
        }
        public async Task<bool> AddMainTopicPost(MainTopicPostViewModel mainTopicPostViewModel)
        {
            MainTopicPost mainTopicPost = new MainTopicPost()
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
            await db.MainTopicPosts.AddAsync(mainTopicPost);
            await db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateMainTopicPost(MainTopicPostViewModel mainTopicPostViewModel)
        {
            MainTopicPost mainTopicPost = await db.MainTopicPosts.FindAsync(mainTopicPostViewModel.MainTopicPostId);
            if (mainTopicPost != null)
            {
                mainTopicPost.MainTopicsIdFK = mainTopicPostViewModel.MainTopicsIdFK;
                mainTopicPost.Title = mainTopicPostViewModel.Title;
                mainTopicPost.Description = mainTopicPostViewModel.Description;
                mainTopicPost.TopicIcon = mainTopicPostViewModel.TopicIcon;
                mainTopicPost.CreatedDate = mainTopicPostViewModel.CreatedDate;
                mainTopicPost.CreatedBy = mainTopicPostViewModel.CreatedBy;
                mainTopicPost.LastUpdatedDate = mainTopicPostViewModel.LastUpdatedDate;
                mainTopicPost.LastUpdatedBy= mainTopicPostViewModel.LastUpdatedBy;
                mainTopicPost.Status = mainTopicPostViewModel.Status;
                await db.SaveChangesAsync();
                return true;
            };
            return false;
        }

        public async Task<List<MainTopicPostViewModel>> GetAllMainTopicPost()
        {
            var result= await db.MainTopicPosts.Select(x => (MainTopicPostViewModel)x).ToListAsync();
            return result;
        }

        //public async Task<List<MainTopicPost>> GetAllMainTopicPost()
        //{
        //    var result = await db.MainTopicPosts.ToListAsync();
        //    return result;
        //}

        public async Task<MainTopicPostViewModel> FindMainTopicPostById(int id)
        {
            var res = await db.MainTopicPosts.FindAsync(id);
            return res;
        }

        public async Task<bool> DeleteMainTopictPostById(int id)
        {
            var mainTopicPostId = db.MainTopicPosts.OrderBy(e => e.MainTopicPostId).Where(a => a.MainTopicPostId == id).FirstOrDefault();
            db.MainTopicPosts.Remove(mainTopicPostId);
            await db.SaveChangesAsync();
            return true;

        }
    }
}
