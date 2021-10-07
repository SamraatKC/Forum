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
        public string Title { get; set; }
        public string Description { get; set; }
        public string TopicIcon { get; set; }
        public int DisplayOrder { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }
        public string Moderator { get; set; }
        public List<MainTopicPost> MainTopicPost { get; set; }
        public ICollection<MainTopic> ChildTopic { get; set; }



    }
}
