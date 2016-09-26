using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotForgottenTwo.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public virtual ApplicationUser UserToBeNotified { get; set; }
        public bool NotificationEmailSent { get; set; }
        public bool NotificationSeenByUser { get; set; }
        public NotificationType NotificationType { get; set; }
        public string NotificationMessage { get; set; }
        public string NotificationSubject { get; set; }
        public virtual ProfilePage NotificationForProfilePage { get; set; } //Nullable
        public string NotificationFromUserId { get; set; } //Nullable
        public bool DontSendEmail { get; set; }
    }

    public enum NotificationType
    {
        NotifyWithMessage,
        NotifyAccessRequired,
        NotifyStoryAddedToExistingBook,
        NotifyWelcomeToTheWebsite,
        NotifyInviteExistingUserToProfilePage,
        NotifyInviteNewUserToProfilePage
    }
}