using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using cms.Models;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
namespace cms.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext context;

        public AccountController()
        {
            context = new ApplicationDbContext();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Name = new SelectList(context.Roles.Where(u => u.Name != "Admin").ToList(), "Name", "Name");
            List<ExpertModel> expertL = new List<ExpertModel>()
            {
                new ExpertModel {Text="Arts", Value=1, IsChecked=false },
                new ExpertModel {Text="Business", Value=2, IsChecked=false },
                new ExpertModel {Text="Culture", Value=3, IsChecked=false },
                new ExpertModel {Text="Ecnomy", Value=4, IsChecked=false },
                new ExpertModel {Text="Education", Value=5, IsChecked=false },
                new ExpertModel {Text="Finance", Value=6, IsChecked=false },
                new ExpertModel {Text="Health", Value=7, IsChecked=false },
                new ExpertModel {Text="Information Technology", Value=8, IsChecked=false },
                new ExpertModel {Text="Health", Value=9, IsChecked=false },
                new ExpertModel {Text="Mathmetics", Value=10, IsChecked=false },
                new ExpertModel {Text="Science", Value=11, IsChecked=false },
                new ExpertModel {Text="Sports", Value=12, IsChecked=false }
            };
            RegisterViewModel model = new RegisterViewModel();
            model.expertList = expertL;
            return View(model);
        }
        
        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
                 var selectedExpert = model.expertList.Where(x => x.IsChecked == true).ToList<ExpertModel>();
                 TempData.Add( "expertList", string.Join(",", selectedExpert.Select(x => x.Text)));
                 

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };
     
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await this.UserManager.AddToRoleAsync(user.Id, model.UserRoles);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    TempData.Add("userid", user.Id);
                    return RedirectToAction("AddExpert", "Account");
                }
                ViewBag.Name = new SelectList(context.Roles.Where(u => u.Name != "Admin").ToList(), "Name", "Name");
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult AddExpert()
        {
            Entities1 db = new Entities1();
            string id = TempData["userid"].ToString();
            AspNetUser aspnetuser = db.AspNetUsers.Find(id);
            aspnetuser.Expert = TempData["expertList"].ToString();
            db.Entry(aspnetuser).State = EntityState.Modified;
            db.SaveChanges();
            TempData.Remove("userid");
            TempData.Remove("expertList");
            return RedirectToAction("Home", "Conferences");
        }

        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Home", "Conferences");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Home", "Conferences");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}