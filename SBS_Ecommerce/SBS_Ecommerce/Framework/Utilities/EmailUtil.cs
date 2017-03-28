using SBS_Ecommerce.Framework.Configurations;
using System;
using System.Net;
using System.Net.Mail;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class EmailUtil
    {
        private const string ClassName = nameof(EmailUtil);
        private MailAddress fromAddress;
        private MailAddress toAddress;
        private SmtpClient smtp;

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
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

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
            LoggingUtil.EndLog(ClassName, methodName);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="toEmail">To email.</param>
        /// <param name="toName">To name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public int SendEmail(string toEmail, string toName, string subject, string body)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            int result = SBSConstants.Success;
            try
            {
                toAddress = new MailAddress(toEmail, toName);
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);                    
                }
            } catch(Exception e)
            {
                result = SBSConstants.Failed;
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return result;
        }
    }
}