using System;
using System.Net.Mail;

namespace TeamGlobal.Infrastructure.Services
{
    public class SmtpHelper : ISmtpHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SmtpClient smtpClient;
        private string subject;
        private string body;
        private string to;

        public SmtpHelper(string to)
        {
            this.to = to;
            smtpClient = new SmtpClient();

        }

        public void SetSubject(string subject)
        {
            this.subject = subject;
        }

        public void SetBody(string body)
        {
            this.body = body;
        }

        public void SendMail()
        {

            try
            {
                MailMessage mail = new MailMessage("noreply@teamglobal.com", to, subject, body);
                mail.From = new MailAddress("noreply@teamglobal.com", "noreply@teamglobal.com");
                mail.IsBodyHtml = true;

                smtpClient.Send(mail);
            }
            catch (System.Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
            }
        }
    }

    public interface ISmtpHelper
    {
        void SetSubject(string subject);

        void SetBody(string body);

        void SendMail();
    }
}