using Forum.Models.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Models.ViewModels
{
    public class MainTopicViewModel
    {
        public int MainTopicId { get; set; }
        public int ThemeIdFK { get; set; }
        public int ParentIdFK { get; set; }
        public string ReferenceLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string Location { get; set; }
        public IFormFile Graphics { get; set; }
        public string TopicIcon { get; set; }
        public string DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }

        public static implicit operator MainTopicViewModel(MainTopic mainTopic)
        {
            MainTopicViewModel mainTopicVM = new MainTopicViewModel();
            mainTopicVM.MainTopicId = mainTopic.MainTopicId;
            mainTopicVM.ThemeIdFK = mainTopic.ThemeIdFK;
            mainTopicVM.ParentIdFK = mainTopic.ParentIdFK;
            mainTopicVM.ReferenceLink = mainTopic.ReferenceLink;
            mainTopicVM.Title = mainTopic.Title;
            mainTopicVM.Description = mainTopic.Description;
            mainTopicVM.TopicIcon = mainTopic.TopicIcon;
            mainTopicVM.DisplayOrder = mainTopic.DisplayOrder;
            mainTopicVM.CreatedDate = mainTopic.CreatedDate;
            mainTopicVM.CreatedBy = mainTopic.CreatedBy;
            mainTopicVM.LastUpdatedDate = mainTopic.LastUpdatedDate;
            mainTopicVM.LastUpdatedBy = mainTopic.LastUpdatedBy;
            mainTopicVM.Status = mainTopic.Status;

            return mainTopicVM;
        }
    }
}
