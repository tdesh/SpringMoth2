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

namespace NotForgottenTwo.Controllers
{
   


    [Authorize]
    public class StoriesController : Controller
    {
        private ApplicationDbContext db { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public StoriesController()
        {
            this.db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
        }



        // GET: Stories
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();
            return View(await db.Stories.Where(s=>s.Owner.Id == currentUserId).ToListAsync());
        }

        // GET: Stories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Story story = await db.Stories.FindAsync(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // GET: Stories/Create
       
        public async Task<ActionResult> Create(int? ProfilePageId)
        {
            //Get Profile

            var Profile = await db.ProfilePages.FindAsync(ProfilePageId);

            ViewBag.ProfilePicture = Profile.ProfilePicture;
            ViewBag.ProfilePageId = ProfilePageId;
            ViewBag.ProfileName = Profile.Name;
            return View();
        }

        // POST: Stories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Story story, int ProfilePageId)
        {
            story.Owner = UserManager.FindById(User.Identity.GetUserId());
            if (ModelState.IsValid)
            {
                var Profile = await db.ProfilePages.FindAsync(ProfilePageId);
                story.ProfilePage = Profile;

                //look for an existing relation and add if not found
                var relation = await db.UserProfilePageRelations.FirstOrDefaultAsync(s => s.Page.Id == ProfilePageId && s.User.Id == story.Owner.Id);
                if(relation==null)
                {
                    //Add a relation
                    var userRelation = new UserProfilePageRelation()
                    {
                        User = story.Owner,
                        Page = Profile
                    };
                    db.UserProfilePageRelations.Add(userRelation);
                }

                db.Stories.Add(story);
                await db.SaveChangesAsync();

                //Notify Other participants
                // The ToList() gets all the participants and avoid lazy loading
                var listOfParticipant = db.UserProfilePageRelations.Where(s => s.Page.Id == ProfilePageId).ToList();

                var NotificationList = new List<Notification>();
                foreach (var participant in listOfParticipant)
                {
                    var notification = new Notification()
                    {
                        NotificationForProfilePage = Profile,
                        UserToBeNotified = participant.User,
                        NotificationFromUserId = story.Owner.Id,
                        NotificationType = NotificationType.NotifyStoryAddedToExistingBook,
                        NotificationSubject = string.Format("{0} added a new story to the book for {1}", story.Owner.Email, Profile.Name),
                        NotificationMessage = string.Format("New Story|{0}", story.Id)
                    };
                    NotificationList.Add(notification);
                }

                db.Notifications.AddRange(NotificationList);
                db.SaveChanges();
                return RedirectToAction("Details", "ProfilePages", new { id = ProfilePageId }); 
            }

            return View(story);
        }

        // GET: Stories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Story story = await db.Stories.FindAsync(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // POST: Stories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Text")] Story story)
        {
            if (ModelState.IsValid)
            {
                db.Entry(story).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(story);
        }

        // GET: Stories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Story story = await db.Stories.FindAsync(id);
            if (story == null)
            {
                return HttpNotFound();
            }
            return View(story);
        }

        // POST: Stories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Story story = await db.Stories.FindAsync(id);
            db.Stories.Remove(story);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
