using EmailSender.ApplicationDBContext;
using EmailSender.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using NToastNotify;
//using System.Net.Mail;

namespace EmailSender.Controllers
{
    public class MessagesController : Controller
    {
        private readonly AppDBContext _appDBContext;
        private readonly IToastNotification _toastNotification;
        public MessagesController(AppDBContext appDBContext, IToastNotification toastNotification)
        {

            _appDBContext = appDBContext;
            _toastNotification = toastNotification;
        }
        /// <summary>
        /// This Method return single View with two sections [ Create Message & List of created Messages ]
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateAndShowMessages()
        {
            var messageObj = new Message();

            var allMesssages = await _appDBContext.Messages.ToListAsync();
            ViewBag.MessagesList = allMesssages;
            return View(messageObj);
        }

        /// <summary>
        /// This method for creating a new message 
        /// </summary>
        /// <param name="message"></param>
        /// <returns> Redirect to the home page </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAndShowMessages(Message message)
        {
            if(!ModelState.IsValid)
            {
                return View(message);
            }

            _appDBContext.Messages.Add(message);
            _appDBContext.SaveChanges();
            _toastNotification.AddSuccessToastMessage("Message Created Successfully");
            return RedirectToAction(nameof(CreateAndShowMessages));
        }

        /// <summary>
        /// this method used to navigate to the Send Email Form with the chosen message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendEmail(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            var messageDetails = await _appDBContext.Messages.FindAsync(id);

            if(messageDetails == null)
            {
                return NotFound();
            }

            var messageInfo = new MailToBeSent
            {
                Id = messageDetails.Id,
                Subject = messageDetails.Subject,
                MessageBody = messageDetails.MessageBody,
                ToEmailAddresses = string.Empty
            };

            return View(messageInfo);
        }

        /// <summary>
        /// This function handle message send
        /// </summary>
        /// <param name="mailObj"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(MailToBeSent mailObj)
        {
            if (!ModelState.IsValid)
            {
                return View(mailObj);
            }


            // >>>> Note: the email [ sdpmail23@gmail.com ] is an email account that I'm using for projects and tests and not my personal email <<<<

            var senderName = "Aman Email System - Ahmed Ashraf";
            var senderMail = "sdpmail23@gmail.com";
            
           // MailboxAddress fromAddress = new MailboxAddress(senderName, senderMail);
            // Add the recipients
            var emailIDsList = mailObj.ToEmailAddresses.Split(',').Where((email)=> email != "" ).ToList();

            MimeMessage message = new MimeMessage();
            // Add the Sender Details
            MailboxAddress fromAddress = new MailboxAddress(senderName, senderMail);
            message.From.Add(fromAddress);
            message.Subject = mailObj.Subject;

            message.Body = new TextPart("plain")
            {
                Text = mailObj.MessageBody,
            };

            foreach (var emailID in emailIDsList)
            {
                message.To.Clear(); // make sure that the TO is empty before adding the new address

                try
                {
                    message.To.Add(MailboxAddress.Parse(emailID));
                    _sendEmailToAddress(message,mailObj);


                }catch (ParseException ex)
                {
                    var AddingEmailsErrorMessage = ex.Message.ToString();
                    _toastNotification.AddErrorToastMessage(AddingEmailsErrorMessage);
                    return View(mailObj); // send the object again 
                }
            }
           

            return RedirectToAction(nameof(CreateAndShowMessages)); // back to the main page
        }


        /// <summary>
        /// This function send the message to the selected Address that is assigned in the MimeMessage Object
        /// </summary>
        /// <param name="message"> Message details [ To, From ,Subject , Body ] </param>
        /// <param name="mailObj"> The mail object submitted in the SendEmail Form </param>
        /// <returns> IActionResult - View() </returns>
        private IActionResult _sendEmailToAddress(MimeMessage message, MailToBeSent mailObj)
        {
            // Configure SMTP Settings
            SmtpClient smtpClient = new SmtpClient();
            var host = "smtp.gmail.com";
            var port = 465; // SMTPS port
            smtpClient.Connect(host, port, true);
            // App Password Generated from the Google Account [ sdpmail23@gmail.com ]
            var appPassword = "yagbdkjtqtuhukvc";
            // message.From[0].Name => return only sender name without email
            // message.From[0] = { SenderName <email> }

            
            string sender = message.From[0].ToString();
            var indexOfTheSmallerSign = sender.IndexOf("<");
            sender= sender.Substring(indexOfTheSmallerSign+1);
            sender = sender.Replace(">","");

            smtpClient.Authenticate(sender, appPassword);

            try
            {
                smtpClient.Send(message);
                _toastNotification.AddSuccessToastMessage("The email has been sent successfully to the recipients");
                return RedirectToAction(nameof(CreateAndShowMessages));
            }
            catch (Exception ex)
            {
                var SMTPErrorMessage = ex.Message.ToString();
                _toastNotification.AddErrorToastMessage(SMTPErrorMessage);
                return View(mailObj);
            }
            finally
            {
                // Close the connection to the server
                smtpClient.Disconnect(true);
                // Dispose smtpClient Obj
                smtpClient.Dispose();
            }
        }



        /// <summary>
        /// This method for deleting a message 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteMessage(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }

            var messageObj = await _appDBContext.Messages.FindAsync(id);
            if(messageObj == null)
            {
                return NotFound();
            }

            _appDBContext.Remove(messageObj);
            _appDBContext.SaveChanges();
            
            return Ok();

        }

    }
}


