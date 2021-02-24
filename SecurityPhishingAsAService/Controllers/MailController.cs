using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SecurityPhishingAsAService.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MailController : ControllerBase
    {
        [HttpPost("SendMailTo/{from}/{subject}/{body}")]
        public bool SendMail(string email, string from, string body, string subject)
        {
            try
            {
                var smtpClient = new SmtpClient("localhost")
                {
                    Port = 25
                };
                smtpClient.Send(from, email, subject, body);
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