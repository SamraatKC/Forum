using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Models.DataModels
{
   public class Channel
    {
        [Key]
        public int ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public string ChannelIconURL { get; set; }
        public string DisplayOrder { get; set; }
        public string ParentChannel { get; set; }
    }
}
