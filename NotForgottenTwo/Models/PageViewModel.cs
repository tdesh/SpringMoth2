using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NotForgottenTwo.Models
{

    
        public class PageViewModel
        {
        }

        public class ProfilePage
        {
            [Key]
            public int Id { get; set; }
            public string ProfilePicture { get; set; }

            public string Name { get; set; }
            public virtual ApplicationUser Owner  { get; set; }

            public virtual List<Story> Stories { get; set; }

            public string ShortIntro { get; set; }

            public virtual List<UserProfilePageRelation> UserProfilePageRelation { get; set; }

    }

        //public class User
        //{
        //    public int id { get; set; }
        //    public string Name { get; set; }
        //    public string Email { get; set; }
        //    public string ProfilePicture { get; set; }

        //}

    public class Story
    {
        [Key]
        public int Id { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public bool ApprovedByAdmin { get; set; }

        public virtual ProfilePage ProfilePage { get; set; }
        public string Title { get; set; }

        public string Text { get; set; }
        public List<string> Photos { get; set; }

    }

    public class UserProfilePageRelation
    {
        [Key]
        public int UserStoryId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ProfilePage Page { get; set; }

        //public bool AccessToBeGranted { get; set; }
    }

    
}