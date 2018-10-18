using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using weddingplanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace weddingplanner.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet("login_page")]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.User.Any(a => a.email == user.email))
                {
                    ModelState.AddModelError("email", "Email already exists!");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    user.password = Hasher.HashPassword(user, user.password);
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    HttpContext.Session.SetInt32("UserID", user.UserId);
                    return RedirectToAction ("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login_User(Login user)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.User.FirstOrDefault(u => u.email == user.email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("email", "Invalid Email");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(user, userInDb.password, user.password);
                
                if(result == 0)
                {
                    ModelState.AddModelError("password", "Invalid Password");
                    return View("Login");
                }
                 
                else
                {
                    return RedirectToAction ("Dashboard");
                }
            }
            else
            {
                return View("Login");
            }
        }
        [HttpGet("clear")]
        public RedirectToActionResult Clear()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            return View(InitializeDashboard());
        }

        [HttpGet("plan")]
        public IActionResult Plan()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.UserId = HttpContext.Session.GetInt32("UserID");
            return View("Plan");
        }

        [HttpPost("create_wedding")]
        public IActionResult Create_Wedding(Wedding wedding)
        {
            if(ModelState.IsValid)
            {
                dbContext.Add(wedding);
                dbContext.SaveChanges();
                var wedding_id = wedding.WeddingId;
                return Redirect($"wedding_info/{wedding_id}");
            }
            if(wedding.created_at < DateTime.Now) {
                ModelState.AddModelError("WeddingDate", "Wedding must be in the future");
            }
            return View("Plan", wedding);
        }
        
        [HttpGet("wedding_info/{id}")]
        public IActionResult Wedding_Info(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Wedding wedding = dbContext.Wedding.Include(p => p.Guest).ThenInclude(a => a.InvitedUser).FirstOrDefault(w => w.WeddingId == id);
            return View("Wedding_Info", wedding);
        }

        [HttpGet("delete/{id}")]
        public RedirectToActionResult Delete(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            Wedding theWedding = dbContext.Wedding.SingleOrDefault(w => w.WeddingId == id);
            dbContext.Wedding.Remove(theWedding);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("RSVP/{id}")]
        public RedirectToActionResult RSVP(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            int? logged_in_user = HttpContext.Session.GetInt32("UserID");
            User currentUser = dbContext.User.SingleOrDefault(u => u.UserId == logged_in_user);
            Wedding currentWedding = dbContext.Wedding.Include(g => g.Guest).ThenInclude(g => g.InvitedUser).SingleOrDefault(w => w.WeddingId == id);
            Guest newGuest = new Guest{UserId=currentUser.UserId, InvitedUser=currentUser, WeddingId=currentWedding.WeddingId, Wedding=currentWedding};
            currentWedding.Guest.Add(newGuest);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("UN-RSVP/{id}")]
        public RedirectToActionResult UNRSVP(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Index");
            }
            int? logged_in_user = HttpContext.Session.GetInt32("UserID");
            User currentUser = dbContext.User.SingleOrDefault(u => u.UserId == logged_in_user);
            Wedding currentWedding = dbContext.Wedding.Include(g => g.Guest).ThenInclude(g => g.InvitedUser).SingleOrDefault(w => w.WeddingId == id);
            Guest invite = dbContext.Guest.SingleOrDefault(i => i.UserId == logged_in_user && i.WeddingId == id);
            currentWedding.Guest.Remove(invite);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        public DashboardModels InitializeDashboard()
        {
            int? logged_in_user = HttpContext.Session.GetInt32("UserID");
            List<Wedding> userJoined = dbContext.Guest.Where(i => i.UserId == logged_in_user).Select(u => u.Wedding).ToList();
            List<Wedding> notJoined = dbContext.Wedding.Except(userJoined).ToList();
            return new DashboardModels
            {
                allWeddings = dbContext.Wedding.Include(w => w.Guest).ToList(),
                User = dbContext.User.Where(u => u.UserId ==logged_in_user).FirstOrDefault(),
                JoinedWeddings = userJoined,
                NotJoinedWeddings = notJoined
            };
        }

    }
}
