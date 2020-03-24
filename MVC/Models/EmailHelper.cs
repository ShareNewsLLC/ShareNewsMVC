using DAL.Helpers;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace MVC.Models
{
    public class EmailHelper
    {
        private readonly string SENDER_EMAIL = "safenewsman@gmail.com";
        private readonly string SENDER_PASS = "SAFE2020news";

        private readonly AuthorHelper authorHelper = new AuthorHelper();
        private readonly EmailCodeHelper emailCodeHelper = new EmailCodeHelper();

        public void SendCode(int AuthorId)
        {
            Author author = authorHelper.GetById(AuthorId);

            // at this point, author must have a new email address to be verified
            author.isEmailConfirmed = false;
            authorHelper.Update(author);

            // invalidate all other previous EmailCodes of same author
            var oldConfirmations = emailCodeHelper.GetAll().Where(ec => ec.AuthorId == author.Id && !ec.isExpired);
            foreach (EmailCode code in oldConfirmations)
            {
                code.isExpired = true;
                emailCodeHelper.Update(code);
            }

            // 6 digit random code
            int confirmation_number = new Random().Next(100000, 999999);

            // save code to db
            emailCodeHelper.Create(new EmailCode() {
                AuthorId            = author.Id,
                Email               = author.Email,
                ConfirmationNumber  = confirmation_number,
                isExpired           = false
            }); 

            // send email to author
            string title = "Confirmation number";
            string body = "Dear " + author.FullName + "\nYour confirmation number is: " + confirmation_number;
            SendMail(author.Email, title, body);
        }

        public void SendMail(string to, string subject, string body)
        {
            // send mail function goes here
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(SENDER_EMAIL);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;

                message.Body = body;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(SENDER_EMAIL, SENDER_PASS);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception)
            {
                // ...
            }
        }

    }
}