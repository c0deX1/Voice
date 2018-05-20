using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Voice.Models;
using System.Security.Cryptography;
using System.Text;

namespace Voice.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        public string Cypher(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                string PasswordHash = Cypher(model.Password);
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Login && u.Password == PasswordHash);
                }
                if (user != null)
                {
                    var cookie = new HttpCookie("FileSave");
                    cookie["Username"] = user.Login;
                    Response.Cookies.Add(cookie);
                    FormsAuthentication.SetAuthCookie(model.Login, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }
        
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                string PasswordHash = Cypher(model.Password);
                User user = null;
                using (UserContext db = new UserContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Login);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (UserContext db = new UserContext())
                    {
                        db.Users.Add(new User { Login = model.Login, Password = PasswordHash, Role = model.Role});
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Login == model.Login && u.Password == PasswordHash).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, true);
                        return RedirectToAction("Information", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        public ActionResult Logoff()
        {
            var cookie = new HttpCookie("FileSave");
            cookie["Username"] = "Guest";
            Response.Cookies.Add(cookie);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}