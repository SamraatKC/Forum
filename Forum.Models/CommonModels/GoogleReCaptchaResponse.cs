using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Models.CommonModels
{
    public class GoogleReCaptchaResponse
    {
        public bool success { get; set; }
        public double score { get; set; }
        public string action { get; set; }
        public DateTime challange_ts { get; set; }
        public string hostname { get; set; }
    }
}
