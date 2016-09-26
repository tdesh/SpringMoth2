using Hangfire;
using NotForgottenTwo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace NotForgottenTwo.Services
{
    public class NotificationService
    {
        [DisableConcurrentExecution(120)]
        public static void SendNextNotification()
        {
            using (var db = new ApplicationDbContext())
            {
                //Get Next Notification to be send from Database
                var notificationToBeSent = db.Notifications.FirstOrDefault(n => !n.NotificationEmailSent && !n.DontSendEmail);

                if (notificationToBeSent == null)
                    return;

                //Send Notification Email
                new EmailService().SendEmail(new NotificationFactory().CreateNotificationEmail(notificationToBeSent));

                //On sucess Mark as Sent
                notificationToBeSent.NotificationEmailSent = true;
                db.Entry(notificationToBeSent).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static Email CreateEmaiToBeSentFromNotification(Notification notification)
        {
            var email = new Email();
            email.FromName = "Admin";
            email.FromAddress = "fromspringmoth@gmail.com";
            email.FromPassword = "5eah0r5e$$";
            email.ToName = notification.UserToBeNotified.Email;
            email.ToAddress = //notification.UserToBeNotified.Email;
            email.Subject = notification.NotificationSubject;
            email.Body = notification.NotificationMessage;
            return email;
        }

        public void AddNotification(Notification notification)
        {
            using (var db = new ApplicationDbContext())
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
        }

    }


    public class NotificationFactory
    {

        public Email CreateNotificationEmail(Notification notification)
        {
            var email = new Email();
            email.FromName = "Admin";
            email.FromAddress = "fromspringmoth@gmail.com";
            email.FromPassword = "5eah0r5e$$";
            email.ToName = notification.UserToBeNotified.Email;
            email.ToAddress = "tejal25@gmail.com"; //notification.UserToBeNotified.Email;

            if (notification.NotificationType == NotificationType.NotifyAccessRequired)
            {
                email.Subject = notification.NotificationSubject;//string.Format("{0} has added a new story to the book for {1}");
                email.Body = GetHtmlBody().Replace(@"&lt;&lt;&lt;SpringMothMessageHeader&gt;&gt;&gt;", notification.NotificationSubject);
                email.Body = email.Body.Replace(@"&lt;&lt;&lt;SpringMothMessageTextAboveButton&gt;&gt;&gt;", "");
                email.Body = email.Body.Replace(@"&lt;&lt;&lt;SpringMothActionButton&gt;&gt;&gt;", "Grant Access to Book");
                email.Body = email.Body.Replace("<<<SpringMothActionButtonLink>>>", "https://www.springmoth.com");
                email.IsEmailHtml = true;
                return email;
            }

            if (notification.NotificationType == NotificationType.NotifyStoryAddedToExistingBook)
            {
                email.Subject = notification.NotificationSubject;//string.Format("{0} has added a new story to the book for {1}");
                email.Body = GetHtmlBody().Replace(@"&lt;&lt;&lt;SpringMothMessageHeader&gt;&gt;&gt;", "A New Story Was Added!");
                email.Body = email.Body.Replace(@"&lt;&lt;&lt;SpringMothMessageTextAboveButton&gt;&gt;&gt;", notification.NotificationSubject);
                email.Body = email.Body.Replace(@"&lt;&lt;&lt;SpringMothActionButton&gt;&gt;&gt;", "Read Now");
                email.Body = email.Body.Replace("<<<SpringMothActionButtonLink>>>", "https://www.springmoth.com");
                email.IsEmailHtml = true;
                return email;
            }


            if (notification.NotificationType == NotificationType.NotifyWelcomeToTheWebsite)
            {
                email.Subject = string.Format("Welcome to SpringMoth!");
                string notificationSpecificEmailMessage = "Personal Message from creators to Welcome you to SpringMoth";
                email.Body = GetHtmlBody().Replace("<<<SpringMothMessage>>>", notificationSpecificEmailMessage);
                return email;
            }

            if (notification.NotificationType == NotificationType.NotifyInviteExistingUserToProfilePage)
            {
                email.Subject = string.Format("{0} invites you to join the book for {1}");
                string notificationSpecificEmailMessage = "Join Now Button";
                email.Body = GetHtmlBody().Replace("<<<SpringMothMessage>>>", notificationSpecificEmailMessage);
                return email;
            }

            if (notification.NotificationType == NotificationType.NotifyInviteNewUserToProfilePage)
            {
                email.Subject = string.Format("{0} invites you to join the book for {1}");
                string notificationSpecificEmailMessage = "Join Now Button + Few lines about SpringMoth";
                email.Body = GetHtmlBody().Replace("<<<SpringMothMessage>>>", notificationSpecificEmailMessage);
                return email;
            }


            return email;

        }


        public string GetHtmlBody()
        {
            //var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "emailTemplate.html";

            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    string result = reader.ReadToEnd();
            //    return result;
            //}

            return System.IO.File.ReadAllText(Path.GetFullPath(HostingEnvironment.MapPath(@"~/Content/emailTemplate.html")));

           // return System.IO.File.ReadAllText(@"C:\Users\tejal\Documents\Visual Studio 2015\Projects\NotForgottenTwo\NotForgottenTwo\Content\emailTemplate.html");
        }

    }









}