    using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Common
{
    public class EmailSentStatus
    {
        public bool IsSent { get; set; }
        public string Message { get; set; }
        private readonly AppSettings appSettings;
        public EmailSentStatus(IOptions<AppSettings> _appSettingOptions)
        {
            this.appSettings = _appSettingOptions.Value;
        }
        public EmailSentStatus(bool _isSent, string _message)
        {
            this.IsSent = _isSent;
            this.Message = _message;
        }
    }
}
