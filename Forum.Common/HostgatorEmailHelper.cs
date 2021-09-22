using Forum.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WanderLust.Common
{
    public class HostgatorEmailHelper
    {
        private AppSettings appSettings;
        public HostgatorEmailHelper(IOptions<AppSettings> _appSettings)
        {
            appSettings = _appSettings.Value;
        }

        public EmailSentStatus SendEmail()
        {
            try
            {
                string subject = "This is a test subject";
                string receiverEmailAddress = "sabeen.khatree13@gmail.com";
                string body = "<html><head></head><body><h1>Hello World!<br/> Message via <span style='color:red'>hostgator</span>.</h1></body></html>";
                List<string> attachmentFilesLocation = null;
                List<string> cc = null;
                MailMessage mailMsg = new MailMessage();
                MailAddress fromAddress = new MailAddress("support@thewanderlustholidays.com", "Wanderlust Holidays");
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


                SmtpClient smtp = new SmtpClient("mail.thewanderlustholidays.com", 26);
                smtp.UseDefaultCredentials = false;
                smtp.Host = "mail.thewanderlustholidays.com";
                smtp.Port = 26;
                smtp.EnableSsl = false;

                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential("support@thewanderlustholidays.com", "******");
                smtp.SendCompleted += Smtp_SendCompleted;
                smtp.SendMailAsync(mailMsg);

            }
            catch (SmtpFailedRecipientsException ex)
            {
                new EmailSentStatus(false, ex.Message);
            }
            return new EmailSentStatus(true, "Email Sent");

        }

        private static void Smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //log Error;
            }
            else
            {
                //log success;
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
