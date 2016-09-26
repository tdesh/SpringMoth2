using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NotForgottenTwo.Models
{
    public class Email
    {
        public string FromAddress { get; set; }
        public string FromName { get; set; }
        public string FromPassword { get; set; }
        public string ToAddress { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public bool IsEmailHtml { get; set; }

    }
}