using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Models.ViewModels
{
    public class MainTopicWithInformationViewModel
    {
        public List<MainTopicViewModel>SubTopics { get; set; }
        public List<TopicInformationViewModel> TopicInformations { get; set; }
    }

}
