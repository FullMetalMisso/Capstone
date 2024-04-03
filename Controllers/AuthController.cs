using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Capstone.Controllers
{
    public class AuthController : Controller
    {
        DBContext db = new DBContext();
        // GET: Auth
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            var loggedUser = db.Users.Where(u=> u.Email == user.Email && u.Password == user.Password).FirstOrDefault();

            if (loggedUser != null)
            {
                FormsAuthentication.SetAuthCookie(loggedUser.UserId.ToString(), true);
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorLogin"] = true;
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}