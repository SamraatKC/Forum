using Forum.Models.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Models
{
    public class TopicInformation
    {
        [Key]
        public int TopicInformationId { get; set; }
        public int MainTopicsIdFK { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string TopicIcon { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string Status { get; set; }
        public MainTopic MainTopic { get; set; }
    }
}
