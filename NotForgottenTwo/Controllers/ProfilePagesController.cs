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
using System.Web.Helpers;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.Core;
using NotForgottenTwo.Services;
using System.Configuration;

namespace NotForgottenTwo.Controllers
{
    [Authorize]
    public class ProfilePagesController : Controller
    {
        
        private ApplicationDbContext db { get; set; }
        protected UserManager<ApplicationUser> UserManager { get; set; }

        private ImageService _imageService { get; set;}
       

        public ProfilePagesController()
        {
            this.db = new ApplicationDbContext();
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(this.db));
            this._imageService = new ImageService();
        }
        
       // [OverrideAuthorization]
        public ActionResult GetPeople(string query)
      {
            return Json(_SearchProfilePages(query), JsonRequestBehavior.AllowGet);
        }


        private List<Autocomplete> _SearchProfilePages(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            try
            {
                var results = (from p in db.ProfilePages
                                   //where (p.Name + " " + p.LastName).Contains(query)
                               where (p.Name).Contains(query)
                               orderby p.Name
                               select p).Take(10).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete person = new Autocomplete();

                    person.Name = r.Name;
                    person.Id = r.Id;
                    people.Add(person);
                }

            }
            catch (EntityCommandExecutionException eceex)
            {
                if (eceex.InnerException != null)
                {
                    throw eceex.InnerException;
                }
                throw;
            }
            catch
            {
                throw;
            }
            return people;
        }

        // GET: ProfilePages
        public async Task<ActionResult> Index()
        {
            ViewBag.BlobLocation = ConfigurationManager.AppSettings.Get("ImageRootPath");
            ViewBag.IsDebugMode = HttpContext.IsDebuggingEnabled;
            var currentUserId = User.Identity.GetUserId();


            //TODO: combine these 2 queries
            var profilelistOwner = (await db.ProfilePages
                                            .Where(s => (s.Owner.Id == currentUserId)
                                                     ).ToListAsync());

            var profilelistParticipant = await (from p in db.ProfilePages
                                                from ur in p.UserProfilePageRelation
                                                where ur.User.Id == currentUserId
                                                select p
                                               
                                            ).ToListAsync();

            foreach (var profile in profilelistParticipant)
            {
                if (!profilelistOwner.Any(s => s.Id == profile.Id))
                {
                    profilelistOwner.Add(profile);
                }
            }
           

            return View(profilelistOwner);
        }


        public async Task<ActionResult> OnlyMyStoryBooks()
        {
            ViewBag.BlobLocation = ConfigurationManager.AppSettings.Get("ImageRootPath");
            ViewBag.IsDebugMode = HttpContext.IsDebuggingEnabled;
            var currentUserId = User.Identity.GetUserId();


            //TODO: combine these 2 queries
            var profilelistOwner = (await db.ProfilePages
                                            .Where(s => (s.Owner.Id == currentUserId)
                                                     ).ToListAsync());
            
            return View("Index",profilelistOwner);
        }




        public ActionResult GetImage(string ImageLocation)
        {
            byte[] image = System.IO.File.ReadAllBytes(ImageLocation);
            return File(image, "image/jpeg");
        }

        // GET: ProfilePages/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //Check if User can access page
            var currentUserId = User.Identity.GetUserId();
           
            ProfilePage profilePage = await db.ProfilePages.FindAsync(id);
            if (profilePage == null)
            {
                return HttpNotFound();
            }
            var allstories = profilePage.Stories;

            ViewBag.AccessToPage = profilePage.Owner.Id == currentUserId ||  db.UserProfilePageRelations.Any(u => u.User.Id == currentUserId && u.Page.Id == id);

            //var currentUserId = User.Identity.GetUserId();
            //return View(await db.Stories.Where(s => s.Owner.Id == currentUserId).ToListAsync());

            return View(profilePage);
        }

        // GET: ProfilePages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfilePages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "Id,ProfilePicture,Name,ShortIntro")] ProfilePage profilePage)
        public async Task<ActionResult> Create(ProfilePage profilePage)
        {
           
            profilePage.Owner = UserManager.FindById(User.Identity.GetUserId());

            //Upload to disk (debug mode)
            if (HttpContext.IsDebuggingEnabled)
            {
                var photo = WebImage.GetImageFromRequest();
                // var image = Request.Files["Image"];

                if (photo != null)
                {
                    var newFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                    var imagePath = @"C:\Notforgotten\ProfilePics\" + newFileName;
                    photo.Save(imagePath);
                    profilePage.ProfilePicture = imagePath;
                }
            }
            else  //Upload to Azure Blob
            {
                HttpPostedFileBase file = Request.Files["Image"];
                var uploadedImage = await _imageService.CreateUploadedImage(file);
                await _imageService.AddImageToBlobStorageAsync(uploadedImage);
                //Have blob URL saved here to render directly
                profilePage.ProfilePicture = uploadedImage.Name;
            }
            

            if (ModelState.IsValid)
            {
                db.ProfilePages.Add(profilePage);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(profilePage);
        }

     

        // GET: ProfilePages/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfilePage profilePage = await db.ProfilePages.FindAsync(id);
            if (profilePage == null)
            {
                return HttpNotFound();
            }
            return View(profilePage);
        }

        // POST: ProfilePages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProfilePicture,Name,ShortIntro")] ProfilePage profilePage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(profilePage).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(profilePage);
        }

        // GET: ProfilePages/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProfilePage profilePage = await db.ProfilePages.FindAsync(id);
            if (profilePage == null)
            {
                return HttpNotFound();
            }
            return View(profilePage);
        }

        // POST: ProfilePages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProfilePage profilePage = await db.ProfilePages.FindAsync(id);
            db.ProfilePages.Remove(profilePage);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public ActionResult JoinABook(int Id)
        {
            //Get Profile Page
            var profilePage =   db.ProfilePages.Find(Id);
            var currentUser = UserManager.FindById(User.Identity.GetUserId());

            if (profilePage==null)
                return Json(new { sucess = false }, JsonRequestBehavior.AllowGet);

            var notification = new Notification();
            notification.NotificationType = NotificationType.NotifyAccessRequired;
            notification.NotificationForProfilePage = profilePage;
            notification.NotificationFromUserId = currentUser.Id;
            notification.NotificationMessage = "AccessRequested";
            notification.NotificationSubject = string.Format("{0} is requesting to join the Book for {1}",currentUser.Email ,profilePage.Name);
            notification.UserToBeNotified = profilePage.Owner;

            db.Notifications.Add(notification);
            db.SaveChanges();

            return Json(new { sucess = true }, JsonRequestBehavior.AllowGet);

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
