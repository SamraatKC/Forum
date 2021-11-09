using Forum.Models.DataModels;
using Forum.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Forum.Models
{
    public class TopicInformation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

        public static implicit operator TopicInformation(TopicInformationViewModel vm)
        {
            TopicInformation ti = new TopicInformation();
            ti.TopicInformationId = vm.TopicInformationId;
            ti.Description = vm.Description;
            //c.GraphicsURL = vm.GraphicsURL;
            ti.TopicIcon = vm.TopicIcon;
           
            ti.Title = vm.Title;


            return ti;
        }
    }
}
