﻿using Forum.Models.DataModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static Forum.Common.Enums;

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
        public int DisplayOrder { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }
        public string Moderator { get; set; }

        public List<SelectListItem> Moderators { get; } = new List<SelectListItem>
    {
        new SelectListItem { Value = "M1", Text = "Moderator1" },
        new SelectListItem { Value = "M2", Text = "Moderator1" },
        new SelectListItem { Value = "M3", Text = "Moderator1"  },
    };

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
            mainTopicVM.Moderator = mainTopic.Moderator;

            return mainTopicVM;
        }
    }
}
