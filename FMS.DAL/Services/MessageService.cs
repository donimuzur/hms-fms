using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract;
using FMS.Logger;
using FMS.Logger.Models;
namespace FMS.DAL.Services
{
    public class MessageService : IMessageService
    {
        private IUnitOfWork _uow;

        public MessageService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public bool SendEmailToList(List<string> to, string subject, string body, bool throwError = false)
        {

            try
            {
                var actualTo = new List<string>();

                actualTo.AddRange(to.Distinct());


                var smtpClient = new SmtpClient();

                var mailMessage = new MailMessage { IsBodyHtml = true };
                actualTo.ForEach(s => mailMessage.To.Add(s.Trim()));

                mailMessage.Body = body;

                mailMessage.Subject = subject;
                //Make sure the client and the message are disposed when the asynch send is done
                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();

                    mailMessage.Dispose();
                };

                var sendEmail = ConfigurationManager.AppSettings["SendEmail"];

                if (sendEmail == "1") { 
                    smtpClient.Send(mailMessage);
                    //smtpClient.SendAsync(mailMessage, null); //Sendasynch doesn't have the time to send in some case, no way to make sure it waits 'till the mail is sent for now.
                }
                return true;
            }
            catch(SmtpException ex)
            {
                CoreLogger.CreateLog(new ExceptionModel
                {
                    NameSpace = "FMS.DAL",
                    ClassName = "MessageService",
                    MethodName = "SendEmailToList",
                    ErrorMessage = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    DateTime = DateTime.Now
                }, System.Web.HttpContext.Current.Request.MapPath("~/Log"));
                return false;

            }
            catch (Exception ex)
            {
                CoreLogger.CreateLog(new ExceptionModel
                {
                    NameSpace = "FMS.DAL",
                    ClassName = "MessageService",
                    MethodName = "SendEmailToList",
                    ErrorMessage = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    DateTime = DateTime.Now
                }, System.Web.HttpContext.Current.Request.MapPath("~/Log"));
                return false;
                //throw new BLLException(ExceptionCodes.BLLExceptions.ServerIsBusy);
            }
        }

        public void SendEmail(string to, string subject, string body, bool throwError = false)
        {
            var actualTo = new List<string> { to };
            SendEmailToList(actualTo, subject, body, throwError);
        }

        public bool SendEmailToListWithCC(List<string> to, List<string> cc, string subject, string body, bool throwError = false, List<string> attachments = null)
        {
            try
            {

                var actualTo = new List<string>();

                actualTo.AddRange(to.Distinct());

                cc = cc.Distinct().ToList();

                var smtpClient = new SmtpClient();

                var mailMessage = new MailMessage { IsBodyHtml = true };
                actualTo.ForEach(s => mailMessage.To.Add(s.Trim()));

                cc.ForEach(s => mailMessage.CC.Add(s.Trim()));

                mailMessage.Body = body;

                mailMessage.Subject = subject;

                if (attachments != null)
                {
                    attachments.ForEach(s => mailMessage.Attachments.Add(new Attachment(s.Trim())));
                }

                //Make sure the client and the message are disposed when the asynch send is done
                smtpClient.SendCompleted += (s, e) =>
                {
                    smtpClient.Dispose();

                    mailMessage.Dispose();
                };

                var sendEmail = ConfigurationManager.AppSettings["SendEmail"];

                if (sendEmail == "1")
                {
                    smtpClient.Send(mailMessage);
                    //smtpClient.SendAsync(mailMessage, null); //Sendasynch doesn't have the time to send in some case, no way to make sure it waits 'till the mail is sent for now.
                }
                throw new Exception("Test Exception Bro!!");
                //return true;
            }
            catch(SmtpException ex)
            {
                CoreLogger.CreateLog(new ExceptionModel
                {
                    NameSpace = "FMS.DAL",
                    ClassName = "MessageService",
                    MethodName = "SendEmailToListWithCC",
                    ErrorMessage = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    DateTime = DateTime.Now
                }, System.Web.HttpContext.Current.Request.MapPath("~/Log"));
                return false;
            }
            catch (Exception ex)
            {
                CoreLogger.CreateLog(new ExceptionModel
                {
                    NameSpace = "FMS.DAL",
                    ClassName = "MessageService",
                    MethodName = "SendEmailToListWithCC",
                    ErrorMessage = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    DateTime = DateTime.Now
                }, System.Web.HttpContext.Current.Request.MapPath("~/Log"));

                //var message = ex.Message;
                return false;
                //throw new BLLException(ExceptionCodes.BLLExceptions.ServerIsBusy);
            }
        }
    }
}
