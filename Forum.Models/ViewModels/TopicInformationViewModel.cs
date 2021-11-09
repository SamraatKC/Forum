using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Models.ViewModels
{
    public class TopicInformationViewModel
    {
        [Key]
        public int TopicInformationId { get; set; }
        public int MainTopicsIdFK { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile Graphics { get; set; }
        public string TopicIcon { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }

        public static implicit operator TopicInformationViewModel(TopicInformation mainTopicPost)
        {
            TopicInformationViewModel mainTopicPostVM = new TopicInformationViewModel();
            mainTopicPostVM.TopicInformationId = mainTopicPost.TopicInformationId;
            mainTopicPostVM.MainTopicsIdFK = mainTopicPost.MainTopicsIdFK;
            mainTopicPostVM.Title = mainTopicPost.Title;
            mainTopicPostVM.Description = mainTopicPost.Description;
            mainTopicPostVM.TopicIcon = mainTopicPost.TopicIcon;
            mainTopicPostVM.CreatedDate = mainTopicPost.CreatedDate;
            mainTopicPostVM.CreatedBy = mainTopicPost.CreatedBy;
            mainTopicPostVM.LastUpdatedDate = mainTopicPost.LastUpdatedDate;
            mainTopicPostVM.LastUpdatedBy = mainTopicPost.LastUpdatedBy;
            mainTopicPostVM.Status = mainTopicPost.Status;
            return mainTopicPostVM;
        }
    }
}
