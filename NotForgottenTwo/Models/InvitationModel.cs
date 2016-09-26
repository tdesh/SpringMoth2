using NotForgottenTwo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotForgottenTwo.Models
{
    public class InvitationViewModel
    {
        [EmailAddress, Required]
        public string Email { get; set; }
        
        [Required]
        public string Message { get; set; }

        public int InvitedToProfilePageId { get; set; }

        public string InvitedToProfilePageName { get; set; }

    }


    public class Invitation
    {
        //Existing user sends invite fromscreen
        //Entry is done in the Invites table

        //On register page - Any new user that registers is checked against this table for email and given access if value present
        //Token is sent with invitation email and that token is used later to give access??

        [Key]
        public int Id { get; set; }

        public ApplicationUser Inviter { get; set; }

        public ProfilePage PageInvitedTo { get; set; }

        public string InviteeEmail { get; set; }

        public string Message { get; set; }

        public DateTime InviteSentDate { get; set; }

        public DateTime? LastReminderDate { get; set; }

        public bool InviteAccepted { get; set; }
    }


   




}