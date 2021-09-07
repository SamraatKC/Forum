using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Forum.Common
{
    public class EmailHelper
    {
        private AppSettings appSettings;
        private ILogger<EmailHelper> logger;
        public EmailHelper(IOptions<AppSettings> _appSettings, ILogger<EmailHelper> _logger)
        {
            appSettings = _appSettings.Value;
            logger = _logger;
        }

        public EmailSentStatus SendEmail(string subject, string receiverEmailAddress, string body, List<string> cc = null, List<string> attachmentFilesLocation = null)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();
                MailAddress fromAddress = new MailAddress(appSettings.EmailSenderAddress, appSettings.EmailSenderDisplayName);
                MailAddress toAddress = new MailAddress(receiverEmailAddress);
                mailMsg.From = fromAddress;
                mailMsg.To.Add(toAddress);
                if (cc != null)
                    foreach (string _cc in cc)
                        mailMsg.CC.Add(_cc);
                mailMsg.Subject = subject;
                mailMsg.IsBodyHtml = true;
                mailMsg.Body = body;
                if (attachmentFilesLocation != null)
                    foreach (string fileLocation in attachmentFilesLocation)
                        mailMsg.Attachments.Add(new Attachment(fileLocation));


                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = appSettings.SMTPHost;
                smtp.Port = appSettings.SMTPPort;
                smtp.EnableSsl = true;

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(appSettings.NetworkCredentialUserName, appSettings.NetworkCredentialPassword);
                smtp.SendCompleted += Smtp_SendCompleted;
                smtp.SendMailAsync(mailMsg);

            }
            catch (SmtpFailedRecipientsException ex)
            {
                new EmailSentStatus(false, ex.Message);
            }
            return new EmailSentStatus(true, "Email Sent");

        }

        private void Smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                logger.LogError(e.Error, e.Error.Message);
            }
            else
            {
                logger.LogInformation("Email Sent");
            }
        }

        public string GetEmailBody(string fileName)
        {
            var runningDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string htmlEmailBody;
            fileName = string.Concat(runningDirectory, fileName);
            using (StreamReader file = new StreamReader(fileName))
            {
                htmlEmailBody = file.ReadToEnd();
            }
            return htmlEmailBody;
        }
    }
}
