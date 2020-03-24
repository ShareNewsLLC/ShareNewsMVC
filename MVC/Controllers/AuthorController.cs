using DAL.Helpers;
using DAL.Models;
using MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class AuthorController : Controller
    {
        private readonly AuthorHelper       authorHelper    = new AuthorHelper();
        private readonly PostHelper         postHelper      = new PostHelper();
        private readonly CategoryHelper     categoryHelper  = new CategoryHelper();
        private readonly EmailHelper        emailHelper     = new EmailHelper();
        private readonly EmailCodeHelper    emailCodeHelper = new EmailCodeHelper();
        private readonly AvatarHelper       avatarHelper    = new AvatarHelper();

        public ActionResult Index()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (!author.isEmailConfirmed)
                return RedirectToAction("ConfirmEmail");

            List<Post> posts = postHelper.GetAll().Where(p => p.AuthorId == author.Id && p.isActive).OrderByDescending(p=>p.DateCreated).ToList();

            ViewBag.Categories = categoryHelper.GetAll().Where(cat => cat.isActive);
            return View(posts);
        }

        public ActionResult InactivePosts()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (!author.isEmailConfirmed)
                return RedirectToAction("ConfirmEmail");

            List<Post> posts = postHelper.GetAll().Where(p => p.AuthorId == author.Id && !p.isActive).OrderByDescending(p=>p.DateCreated).ToList();

            ViewBag.Categories = categoryHelper.GetAll().Where(cat => cat.isActive);
            return View(posts);
        }

        public ActionResult ConfirmEmail()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (author.isEmailConfirmed)
                return RedirectToAction("Index");

            ViewBag.Error = "";
            ViewBag.Email = author.Email;
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmEmail(int? ConfirmationCode)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (author.isEmailConfirmed)
                return RedirectToAction("Index");

            ViewBag.Email = author.Email;

            if (ConfirmationCode == null)
            {
                ViewBag.Error = "Invalid confirmation code!";
                return View();
            }

            if (ConfirmationCode.Value.ToString().Length != 6)
            {
                ViewBag.Error = "Invalid confirmation code!";
                return View();
            }

            EmailCode emailCode = emailCodeHelper.GetAll().SingleOrDefault(e => e.AuthorId == author.Id && e.Email == author.Email && !e.isExpired);

            if(emailCode != null)
            {
                author.isEmailConfirmed = true;
                authorHelper.Update(author);

                emailCode.isExpired = true;
                emailCodeHelper.Update(emailCode);

                return RedirectToAction("Index");
            }
            else
                ViewBag.Error = "Invalid confirmation code!";

            return View();
        }

        public ActionResult ResendCode()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            emailHelper.SendCode(author.Id);

            return RedirectToAction("ConfirmEmail");
        }

        public ActionResult Profile()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (!author.isEmailConfirmed)
                return RedirectToAction("ConfirmEmail");

            return View(author);
        }

        public ActionResult UpdateProfile()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());
            author.Password = "";

            if (!author.isEmailConfirmed)
                return RedirectToAction("ConfirmEmail");

            return View(author);
        }
        
        [HttpPost]
        public ActionResult UpdateProfile(Author a, HttpPostedFileBase imageFile)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (!author.isEmailConfirmed)
                return RedirectToAction("ConfirmEmail");
            
            try
            {
                // check if email is new
                if (author.Email.ToLower() != a.Email.ToLower())
                {
                    author.FullName = a.FullName;
                    author.Email = a.Email;
                    authorHelper.Update(author);
                    // send email confirmation code 
                    emailHelper.SendCode(author.Id);
                }
                
                // set profile picture
                Avatar avatar = avatarHelper.GetAll().SingleOrDefault(ava => ava.AuthorId == author.Id);

                if (imageFile?.ContentLength > 0 && imageFile.ContentType.Contains("image"))
                {
                    using (var stream = new MemoryStream())
                    {
                        imageFile.InputStream.CopyTo(stream);
                        if (avatar != null)
                        {
                            avatar.Source = stream.ToArray();
                            avatarHelper.Update(avatar);
                        }
                        else
                        {
                            avatar = new Avatar() { AuthorId = author.Id, Source = stream.ToArray() };
                            avatarHelper.Create(avatar);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                author.Password = "";
                return View(author);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ChangePassword()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            return View(new PasswordChangeModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel PM)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());

            if (Hash(PM.OldPassword) == author.Password && PM.NewPassword == PM.ConfirmPassword)
            {
                author.Password = Hash(PM.NewPassword);
                authorHelper.Update(author);

                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError("", "Invalid form data!");
                return View(PM);
            }
        }

        public ActionResult GoPublic()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());
            author.isAnonymous = false;
            authorHelper.Update(author);

            return RedirectToAction("Profile");
        }

        public ActionResult GoAnonymous()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!isAdmin() && !isAuthor())
                return RedirectToAction("Logout", "Auth");

            Author author = authorHelper.GetById(GetAuthorId());
            author.isAnonymous = true;
            authorHelper.Update(author);

            return RedirectToAction("Profile");
        }

        public ActionResult Wait()
        {
            return View();
        }

        private string Hash(string str)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private int GetAuthorId()
        {
            return int.Parse(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        private bool LoggedIn()
        {
            return User.Identity.IsAuthenticated;
        }

        private bool isAuthor()
        {
            return ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value == "Author";
        }

        private bool isAdmin()
        {
            return ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value == "Admin";
        }
    }
}