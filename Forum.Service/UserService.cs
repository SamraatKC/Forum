using Forum.Common;
using Forum.Models.CommonModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Forum.Service
{
    public class UserService
    {
        IOptions<AppSettings> appSettings;
        public UserService(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings;
            
        }
        public async Task<GoogleReCaptchaResponse>ReCaptchaVerification(string token)
        {
            GoogleReCaptchaData googleReCaptchaData = new GoogleReCaptchaData
            {
                response = token,
                secret = appSettings.Value.ReCaptchaSecretKey
            };
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={googleReCaptchaData.secret}&response={googleReCaptchaData.response}");
            var captcharesponse = JsonConvert.DeserializeObject<GoogleReCaptchaResponse>(response);
            return captcharesponse;
        }
    }
}
