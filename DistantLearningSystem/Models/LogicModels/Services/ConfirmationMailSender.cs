using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace DistantLearningSystem.Models.LogicModels.Services
{

    //Оповещение о регистрации на мыло
    public class ConfirmationMailSender : ISender
    {
        private readonly string siteMail;
        private readonly string passWord;
        private readonly string host;
        private readonly int port;

        public void NewActive(ref string s)
        {

        }

        public ConfirmationMailSender()
        {
            siteMail = ConfigurationManager.AppSettings["SiteMail"];
            passWord = ConfigurationManager.AppSettings["EmailPassword"];
            host = ConfigurationManager.AppSettings["host"];
            port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
        }

        public bool Send(string subject, string text, string userMail)
        {
            SmtpClient smtpClient = null;
            MailMessage message = null;

            try
            {
                message = new MailMessage(new MailAddress(siteMail), new MailAddress(userMail))
                {
                    Subject = subject,
                    Body = text
                };

                smtpClient = new SmtpClient(host, port)
                {
                    Credentials = new NetworkCredential(siteMail, passWord),
                    EnableSsl = true
                };
                smtpClient.Send(message);
            }
            catch (SmtpException ex)
            {
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (message != null)
                    message.Dispose();
                if (smtpClient != null)
                    smtpClient.Dispose();
            }

            return true;
        }
    }
}