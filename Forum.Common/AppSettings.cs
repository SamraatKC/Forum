using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Common
{
    public class AppSettings
    {
        public string DefaultConnectionString { get; set; }
        public string JwtKey { get; set; }
        public string JwtExpireDays { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
        public string ReCaptchaSiteKey { get; set; }
        public string ReCaptchaSecretKey { get; set; }
        public string EmailSenderAddress { get; set; }
        public string EmailSenderDisplayName { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string NetworkCredentialUserName { get; set; }
        public string NetworkCredentialPassword { get; set; }
        public string EmailTemplate_AccountVerification { get; set; }
        public string EmailTemplate_ForgotPassword { get; set; }
        public string VerificationLink { get; set; }
        public string PasswordReset { get; set; }
        public string AllowedOrigin { get; set; }
        public string UploadTopicIconPath { get; set; }
        public string ConfirmedAccountPostUrl { get; set; }
    }
}
