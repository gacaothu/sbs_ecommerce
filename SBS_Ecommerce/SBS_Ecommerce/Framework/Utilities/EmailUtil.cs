using SBS_Ecommerce.Framework.Configurations;
using System;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web.Configuration;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class EmailUtil
    {
        private const string ClassName = nameof(EmailUtil);
        private MailAddress fromAddress;
        private SmtpClient smtp;

        public EmailUtil()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailUtil"/> class.
        /// </summary>
        /// <param name="fromEmail">From email.</param>
        /// <param name="fromName">From name.</param>
        /// <param name="password">The password.</param>
        /// <param name="host">The host.</param>
        /// <param name="port">The port.</param>
        public EmailUtil(string fromEmail, string fromName, string password, string host, int port)
        {
            fromAddress = new MailAddress(fromEmail, fromName);
            smtp = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password)
            };
        }

        public bool SendListEmail(MailMessage email)
        {
            smtp = new SmtpClient()
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "smtp.gmail.com",
                Port = 587,
                Credentials = new NetworkCredential("notvalue@gmail.com", "chiyeuminhem#89")
            };
            smtp.Send(email);
            return true;

        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="toName">To name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public int SendEmail(string toEmail, string toName, string subject, string body,bool isBodyHtml)
        {
            int result = SBSConstants.Success;
            try
            {
                var toAddress = new MailAddress(toEmail, toName);
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml= isBodyHtml
                })
                {
                    smtp.Send(message);                    
                }
            } catch(Exception e)
            {
                result = SBSConstants.Failed;
            }
            return result;
        }
    }
}