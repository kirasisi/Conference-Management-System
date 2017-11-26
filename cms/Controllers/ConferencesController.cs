using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cms.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using Microsoft.Ajax.Utilities;

namespace cms.Controllers
{
    
    public class ConferencesController : Controller
    {
        private Entities1 db = new Entities1();

        // GET: Conferences
        public ActionResult Index()
        {
            var conferences = db.Conferences.Include(c => c.AspNetUser);
            string currentUserId = User.Identity.GetUserId();
            return View(conferences.Where(m => m.chair_id == currentUserId).ToList());
        }

        [AllowAnonymous]
        public ActionResult IndexForUser()
        {
            var conferences = db.Conferences.Include(c => c.AspNetUser);
            return View(conferences.ToList());
        }

        // GET: Conferences/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conference conference = db.Conferences.Find(id);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }
        [AllowAnonymous]
        public ActionResult AuthorMainPage()
        {
            return View();
        }

        public ActionResult ChairMainPage()
        {
            var conferences = db.Conferences.Include(c => c.AspNetUser);
            string currentUserId = User.Identity.GetUserId();
            List<Paper> pa = new List<Paper>();
             List<int> conList = conferences.Where(m => m.chair_id == currentUserId).Select(a=>a.Id).ToList();
            var papers = db.Papers.Include(p => p.AspNetUser).Include(p => p.Conference);
            foreach(var i in db.Papers)
            {
                foreach(int j in conList)
                {
                    if (i.conference_id == j )
                    {
                        pa.Add(i);
                    }
                }
               
            }
            return View(pa.ToList());
        }
        [AllowAnonymous]
        public ActionResult EditPaper(int paper_id)
        {
            Paper paper = db.Papers.Find(paper_id);
            if (paper == null)
            {
                return HttpNotFound();
            }
            ViewBag.author_id = new SelectList(db.AspNetUsers, "Id", "Email", paper.author_id);
            ViewBag.conference_id = new SelectList(db.Conferences, "Id", "Name", paper.conference_id);
            return View(paper);
        }

        // POST: Papers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPaper([Bind(Include = "Id,Title,Topic,Deadline,No__of_review,author_id,conference_id,Content,final,status")] Paper paper)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paper).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.author_id = new SelectList(db.AspNetUsers, "Id", "Email", paper.author_id);
            ViewBag.conference_id = new SelectList(db.Conferences, "Id", "Name", paper.conference_id);
            return RedirectToAction("ChairMainPage");
        }
        [AllowAnonymous]
        public ActionResult DetailsForAuthor(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conference conference = db.Conferences.Find(id);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }

        // GET: Conferences/Create
        public ActionResult Create()
        {
            AspNetRole role = new AspNetRole();
            Conference model = new Conference();
            AspNetUser user = new AspNetUser();
            string currentUserId = User.Identity.GetUserId();
            try
            {
                
                model.chair_id = currentUserId;
                //ViewBag.chair_id = new SelectList(db.AspNetUsers, "Id", "Email");
                return View(model);
            }
            catch
            {
                return RedirectToAction("AuthorityError");
            }
        }

        public ActionResult AuthorityError()
        {
            return View();
        }

        // POST: Conferences/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Topic,Paper_deadline,Review_deadline,chair_id")] Conference conference)
        {
            if (ModelState.IsValid)
            {
                db.Conferences.Add(conference);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.chair_id = conference.chair_id;
            return View(conference);
        }

        // GET: Conferences/Edit/5
        public ActionResult Edit(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conference conference = db.Conferences.Find(id);
            if (conference == null)
            {
                return HttpNotFound();
            }
            ViewBag.chair_id = new SelectList(db.AspNetUsers, "Id", "Email", conference.chair_id);
            return View(conference);
        }

        // POST: Conferences/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Topic,Paper_deadline,Review_deadline,chair_id")] Conference conference)
        {
            if (ModelState.IsValid)
            {
                db.Entry(conference).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.chair_id = new SelectList(db.AspNetUsers, "Id", "Email", conference.chair_id);
            return View(conference);
        }

        // GET: Conferences/Delete/5
        public ActionResult Delete(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Conference conference = db.Conferences.Find(id);
            if (conference == null)
            {
                return HttpNotFound();
            }
            return View(conference);
        }

        // POST: Conferences/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Conference conference = db.Conferences.Find(id);
            db.Conferences.Remove(conference);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        
        public ActionResult CreatePaper(Paper model, int conference_id)
        {
          
            string currentUserId = User.Identity.GetUserId();
            model.author_id = currentUserId;
            Conference con = db.Conferences.Find(conference_id);
            model.conference_id = con.Id;
            if(System.DateTime.Now>con.Paper_deadline)
            {
                return RedirectToAction("DDLPASS");

            }
            List<Paper> expertL = new List<Paper>()
            {
                
                new Paper {Text="Arts", Value=1 },
                new Paper {Text="Business", Value=2},
                new Paper {Text="Culture", Value=3},
                new Paper {Text="Ecnomy", Value=4},
                new Paper {Text="Education", Value=5},
                new Paper {Text="Finance", Value=6 },
                new Paper {Text="Health", Value=7},
                new Paper {Text="Information Technology", Value=8},
                new Paper {Text="Health", Value=9},
                new Paper {Text="Mathmetics", Value=10},
                new Paper {Text="Science", Value=11},
                new Paper {Text="Sports", Value=12}
            };
            
            model.expertList = expertL;
            
            return View(model);
        }

        public ActionResult DDLPASS()
        {
            return View();
        }

        // POST: Papers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaper([Bind(Include = "Id,Title,Topic,No__of_review,author_id,conference_id,Content,final,status")] Paper paper,string[] expert)
        {
            if (ModelState.IsValid)
            {

                try
                {

                    var selectedExpert = expert.ToList();
                    
                    paper.Topic = string.Join(",", selectedExpert);
                    paper.No__of_review = 0;
                    paper.status = "Submitted";
                    db.Papers.Add(paper);
                    db.SaveChanges();
                    return RedirectToAction("PaperIndex");
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }

            }
            ViewBag.author_id = paper.AspNetUser.Id;
            ViewBag.conference_id = paper.Conference.Id;
            return RedirectToAction("PaperIndex");
        }
        [AllowAnonymous]
        public ActionResult PaperIndex()
        {                                  
            var papers = db.Papers.Include(p => p.AspNetUser).Include(p => p.Conference);
            string currentUserId = User.Identity.GetUserId();
            return View(papers.Where(m => m.author_id == currentUserId).ToList());
        }
        public ActionResult DeletePaper(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paper paper = db.Papers.Find(id);
            if (paper == null)
            {
                return HttpNotFound();
            }
            return View(paper);
        }

        public ActionResult PaperDeatils(int id)
        {
            if (id < 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paper paper = db.Papers.Find(id);
            if (paper == null)
            {
                return HttpNotFound();
            }
            return View(paper);
        }

        // POST: Papers/Delete/5
        [AllowAnonymous]
        [HttpPost, ActionName("DeletePaper")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed2(Paper model, string id)
        {
            Paper paper = db.Papers.Find(id);
            db.Papers.Remove(paper);
            db.SaveChanges();
            return RedirectToAction("PaperIndex");
        }

        

       
        public ActionResult AssignReviewerSingle(int PaperId, AssignPaper m)
        {
            Paper paper = db.Papers.Find(PaperId);
            
            m.PaperId = paper.Id;

            TempData.Add("I", m.PaperId);
            var users = from u in db.AspNetUsers
                        where u.AspNetRoles.Any(r => r.Name == "Reviewer")
                        select u;

            return View(users.ToList());
        }

        [HttpPost]
        public ActionResult AssignReviewerSingle(AssignPaper assign, AspNetUser user)
        {

            //db.AssignPapers.Add(assign);
            //db.SaveChanges();

            return RedirectToAction("ChairMainPage");
        }

        //[HttpPost]
        public ActionResult assign([Bind(Include = "Id,PaperId,ReviewerId")]AssignPaper assign ,string Id)
        {
            
            
            assign.PaperId = Convert.ToInt32(TempData["I"].ToString());
            Paper paper = db.Papers.Find(assign.PaperId);
            if (paper.No__of_review > 4)
            {
                RedirectToAction("assignFail");
            }
            AspNetUser user = db.AspNetUsers.Find(Id);
            assign.ReviewerId = user.Id;
           
            db.AssignPapers.Add(assign);
            db.SaveChanges();
            TempData.Remove("I");
            return RedirectToAction("ChairMainPage");
        }

        public ActionResult assignFail()
        {
            return View();
        }



        public ActionResult ReviewListForAuthor()
        {
            var papers = db.Papers.Include(p => p.AspNetUser).Include(p => p.Conference);
            string currentUserId = User.Identity.GetUserId();
            List<Review> re = new List<Review>();
            List<int> pa = papers.Where(m => m.author_id == currentUserId).Select(a => a.Id).ToList();
            var reviewes = db.Reviews.Include(p => p.AspNetUser);
            foreach (var i in db.Reviews)
            {
                foreach (int j in pa)
                {
                    if (i.paper_id == j)
                    {
                        re.Add(i);
                    }
                }
            }
            

         
            return View(re.ToList());

        }

        public ActionResult ReviewIndexForChair()
        {
            var conferences = db.Conferences.Include(c => c.AspNetUser);
            string currentUserId = User.Identity.GetUserId();
            List<Paper> pa = new List<Paper>();
            List<int> conList = conferences.Where(m => m.chair_id == currentUserId).Select(a => a.Id).ToList();
            var papers = db.Papers.Include(p => p.AspNetUser).Include(p => p.Conference);
            foreach (var i in db.Papers)
            {
                foreach (int j in conList)
                {
                    if (i.conference_id == j)
                    {
                        pa.Add(i);
                    }
                }

            }
            List<Review> re = new List<Review>();
            foreach(var i in pa)
            {
                foreach(var r in db.Reviews)
                {
                    if(r.paper_id == i.Id)
                    {
                        re.Add(r);
                    }
                }
            }


            return View(re.ToList());
        }
        public ActionResult AdminConference()
        {
            var conferences = db.Conferences.Include(c => c.AspNetUser);
            return View(conferences.ToList());
        }

        //For Admin
        public ActionResult AdminPaper()
        {
            var papers = db.Papers.Include(p => p.AspNetUser).Include(p => p.Conference);
            return View(papers.ToList());
        }

        public ActionResult AdminReview()
        {
            var reviews = db.Reviews.Include(p => p.AspNetUser).Include(p => p.Paper);
            return View(reviews.ToList());
        }

        public ActionResult AdminUser()
        {
            return View();
        }

        public ActionResult AdminUserAuthorList()
        {
            var users = from u in db.AspNetUsers
                        where u.AspNetRoles.Any(r => r.Name == "Author")
                        select u;
            return View(users);
        }
        public ActionResult AdminUserChairList()
        {
            var users = from u in db.AspNetUsers
                        where u.AspNetRoles.Any(r => r.Name == "Chair")
                        select u;
            return View(users);
        }
        public ActionResult AdminUserReviewerList()
        {
            var users = from u in db.AspNetUsers
                        where u.AspNetRoles.Any(r => r.Name == "Reviewer")
                        select u;
            return View(users);
        }

        public ActionResult PaperDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paper paper = db.Papers.Find(id);
            if (paper == null)
            {
                return HttpNotFound();
            }
            return View(paper);
        }

        public ActionResult EditReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.reviewer_id = new SelectList(db.AspNetUsers, "Id", "UserName", review.reviewer_id);
            ViewBag.paper_id = new SelectList(db.Papers, "Id", "Title", review.paper_id);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReview([Bind(Include = "Id,Title,reviewer_id,Deadline,Evaluation,Rating,paper_id,assign_id")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminReview");
            }
            ViewBag.reviewer_id = new SelectList(db.AspNetUsers, "Id", "UserName", review.reviewer_id);
            ViewBag.paper_id = new SelectList(db.Papers, "Id", "Title", review.paper_id);
            return View(review);
        }

        public ActionResult EditReviewForChair(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.reviewer_id = new SelectList(db.AspNetUsers, "Id", "UserName", review.reviewer_id);
            ViewBag.paper_id = new SelectList(db.Papers, "Id", "Title", review.paper_id);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditReviewForChair([Bind(Include = "Id,Title,reviewer_id,Deadline,Evaluation,Rating,paper_id,assign_id,status")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ReviewIndexForChair");
            }
            ViewBag.reviewer_id = new SelectList(db.AspNetUsers, "Id", "UserName", review.reviewer_id);
            ViewBag.paper_id = new SelectList(db.Papers, "Id", "Title", review.paper_id);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public ActionResult DeleteReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("DeleteReview")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedReview(int id)
        {
            Review review = db.Reviews.Find(id);
            db.Reviews.Remove(review);
            db.SaveChanges();
            return RedirectToAction("AdminReview");
        }


        public ActionResult DetailsReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        public ActionResult UserEdit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UserEdit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,Expert")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("AdminUser");
            }
            return View(aspNetUser);
        }
        public ActionResult UserDelete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("UserDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedUser(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("AdminUser");
        }
        public ActionResult UserDetails(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        //For Reviewer
        public ActionResult AssignedPaperIndex()
        {
            string currentUserId = User.Identity.GetUserId();
            var assignpapers = db.AssignPapers.Include(r => r.AspNetUser).Include(r => r.Paper).Where(p => p.ReviewerId == currentUserId);
            return View(assignpapers.ToList());

        }

        public ActionResult AssignedPaperDetails(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paper paper = db.Papers.Find(id);
            if (paper == null)
            {
                return HttpNotFound();
            }
            TempData.Add("Paperid", id);
            return View(paper);
        }

        [Authorize]
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult CreateReview()
        {
            int id = Convert.ToInt32(TempData["Paperid"].ToString());
            foreach (var i in db.Reviews)
            {
                if (i.paper_id == id && i.reviewer_id == User.Identity.GetUserId())
                {
                    return RedirectToAction("assignFail");
                }
            }
            string currentUserId = User.Identity.GetUserId();
            Paper paper = db.Papers.Find(id);
            Conference con = db.Conferences.Find(paper.conference_id);
           
            if (System.DateTime.Now > con.Paper_deadline)
            {
                return RedirectToAction("DDLPASS");

            }

            Review model = new Review();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateReview([Bind(Include = "Id,Title,reviewer_id,Deadline,Evaluation,Rating,paper_id,assign_id,status")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.paper_id = Convert.ToInt32(TempData["Paperid"].ToString());
                review.reviewer_id = User.Identity.GetUserId();
                review.status = "Not yet accepted";
                db.Reviews.Add(review);
                Paper paper = db.Papers.Find(review.paper_id);
                paper.No__of_review = paper.No__of_review + 1;
                db.Entry(paper).State = EntityState.Modified;
                db.SaveChanges();
                TempData.Remove("Paperid");
                return RedirectToAction("AssignedPaperIndex");
            }
            return View(review);
        }


        //Index of my review
        public ActionResult ReviewerReview()
        {
            string currentUserid = User.Identity.GetUserId();
            var reviews = db.Reviews.Include(r => r.AspNetUser).Include(r => r.Paper).Where(p => p.reviewer_id == currentUserid);
            return View(reviews.ToList());
        }

        public ActionResult ReviewerEditReview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReviewerEditReview([Bind(Include = "Id,Title,reviewer_id,Deadline,Evaluation,Rating,paper_id,assign_id")] Review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ReviewerReview");
            }
            return View(review);
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

