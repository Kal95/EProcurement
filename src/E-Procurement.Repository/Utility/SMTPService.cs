
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.Utility.Models;
using E_Procurement.Repository.EmailLogRepo;
using E_Procurement.Data.Entity;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace E_Procurement.Repository.Utility
{
    public class SMTPService : ISMTPService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IEmailSentLog _emailSentLogRepo;

       // private IConfiguration _config;
        public SMTPService(
           IOptions<EmailSettings> emailSettings, IEmailSentLog emailSentLogRepo)
        {
            _emailSettings = emailSettings.Value;
           // _config = config;
            _emailSentLogRepo = emailSentLogRepo;
        }




        public async Task SendEmailAsync(string email, string subject, string message, string attachedfiles)
        {
            try
            {
                //var mimeMessage = new MimeMessage();

                //mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                //mimeMessage.To.Add(new MailboxAddress(email));
                //mimeMessage.Subject = subject;

                //mimeMessage.Body = new TextPart("html")
                //{
                //    Text = message
                //};
                 //var _emailSettings =  _config.GetSection("EmailSettings").Value;

                    //IOptions<EmailSettings> _emailSettings = new IOptions<EmailSettings>(_config.GetSection("EmailSettings"));

               // string fileUrl = _config.GetSection("GoSmarticleEndPoint:filesUrl").Value;

                MailMessage mm = new MailMessage();

                foreach (string n in email.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(n)) mm.To.Add(new MailAddress(n.Trim()));
                }
                mm.Sender = new MailAddress(_emailSettings.SenderName);
                mm.From = new MailAddress(_emailSettings.SenderName);
                mm.Subject = subject;
                mm.Body = message;

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                //attachements
                if (!string.IsNullOrWhiteSpace(attachedfiles))
                {
                    foreach (string attachm in attachedfiles.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(attachm)) mm.Attachments.Add(new System.Net.Mail.Attachment(attachm));
                    }
                }



                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;




                EmailSentLog emailLog = new EmailSentLog
                {
                    EmailContent = message,
                    RecipientEmail = email
                };


            //    using (var client = new SmtpClient())
            //    {

            //        client.Port = _emailSettings.MailPort;
            //        client.Host = _emailSettings.MailServer;
            //        client.EnableSsl = _emailSettings.SSl;   //true
            //        client.Timeout = 20000;
            //        client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //        client.UseDefaultCredentials = false;
            //        client.Credentials = new System.Net.NetworkCredential(_emailSettings.Sender, _emailSettings.Password);


            //        // Note: only needed if the SMTP server requires authentication
            //        //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
            //        try { 
            //            await Task.Run(() =>
            //               client.Send(mm)
            //            );
            //        }
            //        catch (SmtpFailedRecipientsException ex)
            //        {
            //            for (int i = 0; i < ex.InnerExceptions.Length; i++)
            //            {
            //                SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
            //                if (status == SmtpStatusCode.MailboxBusy ||
            //                    status == SmtpStatusCode.MailboxUnavailable)
            //                {
            //                    //Console.WriteLine("Delivery failed - retrying in 5 seconds.");
            //                    //System.Threading.Thread.Sleep(5000);
            //                    client.Send(mm);
            //                }
            //                else
            //                {
            //                    //Console.WriteLine("Failed to deliver message to {0}",
            //                    //    ex.InnerExceptions[i].FailedRecipient);

            //                    emailLog.Status = "Failed";
            //                }
            //            }
            //        }

            //       await  _emailSentLogRepo.LogEmailAsync(emailLog);
            //    // await client.DisconnectAsync(true);
            //}

                var client = new SendGridClient(_emailSettings.Password);
                ////send an email message using the SendGrid Web API with a console application.  
                //var msgs = new SendGridMessage()
                //{
                //    From = new EmailAddress(_emailSettings.SenderName),
                //    Subject = subject,
                //    TemplateId = "fb09a5fb-8bc3-4183-b648-dc6d48axxxxx",
                //    ////If you have html ready and dont want to use Template's   
                //    //PlainTextContent = "Hello, Email!",  
                //    //HtmlContent = "<strong>Hello, Email!</strong>",  
                //};
                var msg = new SendGridMessage();

                msg.SetFrom(new EmailAddress(_emailSettings.SenderName, "E-Procurement Team"));
                msg.SetSubject(subject);

                msg.AddContent(MimeType.Text, message);
                msg.AddContent(MimeType.Html, message);
                //if you have multiple reciepents to send mail  
                msg.AddTo(new EmailAddress(email));
                //If you have attachment  
                var attach = new SendGrid.Helpers.Mail.Attachment();
                //attach.Content = Convert.ToBase64String("rawValues");  
                //attach.Type = "image/png";
                //attach.Filename = "hulk.png";
                //attach.Disposition = "inline";
                //attach.ContentId = "hulk2";
                msg.AddAttachment(attach.Filename, attach.Content, attach.Type, attach.Disposition, attach.ContentId);  
                //Set footer as per your requirement  
               // msgs.SetFooterSetting(true, "<strong>Regards,</strong><b> Pankaj Sapkal", "Pankaj");
                //Tracking (Appends an invisible image to HTML emails to track emails that have been opened)  
                //msgs.SetClickTracking(true, true);  
                var responses = await client.SendEmailAsync(msg);

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }


        public async Task SendEmailTransactionAsync(string email, string subject, string message, string attachedfiles)
        {
            try
            {
                //var mimeMessage = new MimeMessage();

                //mimeMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.Sender));
                //mimeMessage.To.Add(new MailboxAddress(email));
                //mimeMessage.Subject = subject;

                //mimeMessage.Body = new TextPart("html")
                //{
                //    Text = message
                //};
                //var _emailSettings =  _config.GetSection("EmailSettings").Value;

                //IOptions<EmailSettings> _emailSettings = new IOptions<EmailSettings>(_config.GetSection("EmailSettings"));

                // string fileUrl = _config.GetSection("GoSmarticleEndPoint:filesUrl").Value;

                MailMessage mm = new MailMessage();

                foreach (string n in email.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(n)) mm.To.Add(new MailAddress(n.Trim()));
                }
                mm.Sender = new MailAddress(_emailSettings.SenderName);
                mm.From = new MailAddress(_emailSettings.SenderName);
                mm.Subject = subject;
                mm.Body = message;

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

                //attachements
                if (!string.IsNullOrWhiteSpace(attachedfiles))
                {
                    foreach (string attachm in attachedfiles.Split(';'))
                    {
                        if (!string.IsNullOrEmpty(attachm)) mm.Attachments.Add(new System.Net.Mail.Attachment(attachm));
                    }
                }

                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.IsBodyHtml = true;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;


                EmailSentLog emailLog = new EmailSentLog
                {
                    EmailContent = message,
                    RecipientEmail = email
                };


                using (var client = new SmtpClient())
                {

                    client.Port = _emailSettings.MailPort;
                    client.Host = _emailSettings.MailServer;
                    client.EnableSsl = _emailSettings.SSl;   //true
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential(_emailSettings.Sender, _emailSettings.Password);


                    // Note: only needed if the SMTP server requires authentication
                    //await client.AuthenticateAsync(_emailSettings.Sender, _emailSettings.Password);
                    try
                    {
                        await Task.Run(() =>
                           client.Send(mm)
                        );
                    }
                    catch (SmtpFailedRecipientsException ex)
                    {
                        for (int i = 0; i < ex.InnerExceptions.Length; i++)
                        {
                            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                            if (status == SmtpStatusCode.MailboxBusy ||
                                status == SmtpStatusCode.MailboxUnavailable)
                            {
                                //Console.WriteLine("Delivery failed - retrying in 5 seconds.");
                                //System.Threading.Thread.Sleep(5000);
                                client.Send(mm);
                            }
                            else
                            {
                                //Console.WriteLine("Failed to deliver message to {0}",
                                //    ex.InnerExceptions[i].FailedRecipient);

                                emailLog.Status = "Failed";
                            }
                        }
                    }

                    await _emailSentLogRepo.LogEmailTransactionAsync(emailLog);
                    // await client.DisconnectAsync(true);
                }

            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }
        }

    }
}
