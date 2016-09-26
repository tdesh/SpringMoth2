using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using NotForgottenTwo.Models;

namespace NotForgottenTwo.Services
{

    public class EmailService
    {
        public void SendEmail(Email email)
        {
            var fromAddress = new MailAddress(email.FromAddress, email.FromName);
            var toAddress = new MailAddress(email.ToAddress, email.ToName);
            var fromPassword = email.FromPassword;
            var subject = email.Subject;
            var body = email.Body;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = email.IsEmailHtml
            })
            {
                smtp.Send(message);
            }
        }

        public static Email GetTestEmail()
        {
            return new Models.Email()
            {
                FromName = "Admin",
                FromAddress = "fromspringmoth@gmail.com",
                FromPassword = "AddPassword",
                ToName = "Tejal",
                ToAddress = "tejal25@gmail.com",
                Subject = "Random Subject",//string.Format("{0} invites you to share a story about {1}", invitation.Inviter.UserName, invitation.PageInvitedTo.Name);
                Body = "Test Email Sent"
            };
        }
    }
}