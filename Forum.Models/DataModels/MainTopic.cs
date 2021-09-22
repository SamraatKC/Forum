using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Forum.Models.DataModels
{
   public class MainTopic
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MainTopicId { get; set; }
        public int ThemeIdFK { get; set; }
        public int ParentIdFK { get; set; }
        public string ReferenceLink { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public string Location { get; set; }
        public string TopicIcon { get; set; }
        public string DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }
        public List<MainTopicPost> MainTopicPost { get; set; }


    }
}
