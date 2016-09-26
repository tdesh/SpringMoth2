using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NotForgottenTwo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Hangfire;

namespace NotForgottenTwo.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public InvitationsController()
        {
            this.db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
        }

        // GET: Invitations
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();
            return View(await db.Invitations.Where(s => s.Inviter.Id == currentUserId).ToListAsync());
        }

        // GET: Invitations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( InvitationViewModel invitationViewModel)
        {
            if (ModelState.IsValid)
            {
                var inviter = UserManager.FindById(User.Identity.GetUserId());
                var pageInvitedto = db.ProfilePages.FirstOrDefault(s => s.Id == invitationViewModel.InvitedToProfilePageId);

                var invitation = new Invitation()
                {
                    Inviter = inviter,
                    InviteeEmail = invitationViewModel.Email,
                    PageInvitedTo = pageInvitedto,
                    InviteSentDate = DateTime.UtcNow,
                    Message = invitationViewModel.Message
                };

                db.Invitations.Add(invitation);
                db.SaveChanges();

                //Send invitation Email 
                BackgroundJob.Enqueue(() => SendInvitaion(invitation.Id));
              
                return Json(new { sucess = true }, JsonRequestBehavior.AllowGet);
            }

            //TODO: Return error
            return RedirectToAction("InvitaionSent");
        }

        public static void SendInvitaion(int invitationId)
        {
            using (var db = new ApplicationDbContext())
            {
                var invitation = db.Invitations.FirstOrDefault(s => s.Id == invitationId);

                var email = new Email()
                {
                    FromName = "Admin",
                    FromAddress = "fromspringmoth@gmail.com",
                    FromPassword = "Add Password",
                    ToName = invitation.InviteeEmail,
                    ToAddress = invitation.InviteeEmail,
                    Subject = "Random Subject",//string.Format("{0} invites you to share a story about {1}", invitation.Inviter.UserName, invitation.PageInvitedTo.Name);
                    Body = invitation.Message,
                };

                var emailService = new NotForgottenTwo.Services.EmailService();
                emailService.SendEmail(email);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
