using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Models.ViewModels
{
    public class ChannelViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ChannelIcon { get; set; }
        public string ChannelIconURL { get; set; }
        public string Location { get; set; }
        public string DisplayOrder { get; set; }
        public string ParentChannel { get; set; }
    }
}
