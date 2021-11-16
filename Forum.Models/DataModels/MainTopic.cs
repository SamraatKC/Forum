using Forum.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;
using static Forum.Common.Enums;

namespace Forum.Models.DataModels
{
   public class MainTopic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MainTopicId { get; set; }
        public int ThemeIdFK { get; set; }
        [ForeignKey("MainTopicId")]

        public MainTopic Parent { get; set; }
        public int? ParentIdFK { get; set; }
        public string ReferenceLink { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string TopicIcon { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }
        public string Moderator { get; set; }
        public List<TopicInformation> MainTopicPost { get; set; }
        public ICollection<MainTopic> ChildTopic { get; set; }


        public static implicit operator MainTopic(MainTopicViewModel vm)
        {
            MainTopic mt = new MainTopic();
            mt.MainTopicId = vm.MainTopicId;
            mt.Description = vm.Description;
            //c.GraphicsURL = vm.GraphicsURL;
            mt.TopicIcon = vm.TopicIcon;
            mt.ParentIdFK = vm.ParentIdFK;
            mt.DisplayOrder = vm.DisplayOrder;
            mt.Topic = vm.Topic;
            

            return mt;
        }


    }
}
