using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityPhishingAsAService.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MailController : ControllerBase
    {
        [HttpPost("SendMailTo/{type}")]
        public bool SendMail(string email, string from, string displayname, string subject, int type)
        {
            try
            {
                
                string html = System.IO.File.ReadAllText(String.Format("C:\\Developer\\Projects\\SecurityPhishingAsAService\\SecurityPhishingAsAService\\Templates\\Template{0}.html",type));
                var smtpClient = new SmtpClient("localhost")
                {
                    Port = 25
                };

                MailMessage msg = new MailMessage();
                //Console.WriteLine(html);
                msg.Body = html;
                msg.To.Add(email);
                msg.Subject = subject;
                msg.From = new MailAddress(from, displayname);
                //msg.Sender = new MailAddress(from, displayname);
                msg.IsBodyHtml = true;
                smtpClient.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }
            
        }
        [HttpPost("SendMailToBulk/{from}/{subject}/{body}")]
        public bool SendMailInBulk(List<string> emailList, string from, string body, string subject)
        {
            try
            {
                foreach (var email in emailList)
                {
                    try
                    {
                        var smtpClient = new SmtpClient("localhost")
                        {
                            Port = 25
                        };
                        smtpClient.Send(from, email, subject, body);
                    }
                    catch
                    {
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
 
        }
        
    }
}