using DAL.Helpers;
using DAL.Models;
using MVC.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly PostHelper     postHelper      = new PostHelper();
        private readonly CategoryHelper categoryHelper  = new CategoryHelper();
        private readonly AuthorHelper   authorHelper    = new AuthorHelper();
        private readonly AdminHelper    adminHelper     = new AdminHelper();

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var posts = from p in postHelper.GetAll() select p;

            if (!string.IsNullOrEmpty(searchString))
                posts = posts.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));

            switch (sortOrder)
            {
                case "title_desc":
                    posts = posts.OrderByDescending(p => p.Title);
                    break;
                case "Status":
                    posts = posts.OrderBy(p => p.isActive);
                    break;
                case "status_desc":
                    posts = posts.OrderByDescending(p => p.isActive);
                    break;
                default:
                    posts = posts.OrderBy(p => p.Title);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult DeactivatePost(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            Post post = postHelper.GetById(id);

            post.isActive = false;
            postHelper.Update(post);

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult ActivatePost(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            Post post = postHelper.GetById(id);

            post.isActive = true;
            postHelper.Update(post);

            return RedirectToAction("Index", "Admin");
        }

        public ActionResult Authors(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var authors = from a in authorHelper.GetAll() select a;

            if (!string.IsNullOrEmpty(searchString))
                authors = authors.Where(a => a.Email.ToLower().Contains(searchString.ToLower()) || a.FullName.ToLower().Contains(searchString.ToLower()));

            switch (sortOrder)
            {
                case "title_desc":
                    authors = authors.OrderByDescending(a => a.FullName);
                    break;
                case "Status":
                    authors = authors.OrderBy(a => a.isVerified);
                    break;
                case "status_desc":
                    authors = authors.OrderByDescending(a => a.isVerified);
                    break;
                default:
                    authors = authors.OrderBy(a => a.FullName);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(authors.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult VerifyAuthor(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                Author author = authorHelper.GetById(id);
                author.isVerified = true;
                authorHelper.Update(author);
            }
            catch (Exception) { }

            return RedirectToAction("Authors");
        }

        public ActionResult BanAuthor(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                Author author = authorHelper.GetById(id);
                author.isVerified = false;
                authorHelper.Update(author);
            }
            catch (Exception) { }

            return RedirectToAction("Authors");
        }

        public ActionResult Categories()
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            return View(categoryHelper.GetAll());
        }

        public ActionResult CreateCategory()
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            return View(new Category());
        }

        [HttpPost]
        public ActionResult CreateCategory(Category category)
        {
            try
            {
                categoryHelper.Create(category);
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(category);
            }
        }

        public ActionResult EditCategory(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            return View(categoryHelper.GetById(id));
        }

        [HttpPost]
        public ActionResult EditCategory(Category category)
        {
            try
            {
                categoryHelper.Update(category);
                return RedirectToAction("Categories");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(category);
            }
        }

        public ActionResult DeleteCategory(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                categoryHelper.Delete(id);
            }
            catch (Exception) { }

            return RedirectToAction("Categories");
        }

        public ActionResult ActivateCategory(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                Category cat = categoryHelper.GetById(id);
                cat.isActive = true;
                categoryHelper.Update(cat);
            }
            catch (Exception) { }

            return RedirectToAction("Categories");
        }

        public ActionResult DeactivateCategory(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                Category cat = categoryHelper.GetById(id);
                cat.isActive = false;
                categoryHelper.Update(cat);
            }
            catch (Exception) { }

            return RedirectToAction("Categories");
        }

        public ActionResult Profile()
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            Admin admin = adminHelper.GetById(GetAuthorId());

            return View(admin);
        }

        public ActionResult EditProfile()
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            Admin admin = adminHelper.GetById(GetAuthorId());

            return View(admin);
        }

        [HttpPost]
        public ActionResult EditProfile(Admin admin)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            try
            {
                Admin ad = adminHelper.GetById(GetAuthorId());
                ad.Email = admin.Email;
                ad.FullName = admin.FullName;
                adminHelper.Update(ad);

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(admin);
            }
        }

        public ActionResult EditPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditPassword(PasswordChangeModel PM)
        {
            if (!LoggedIn())
                return RedirectToAction("LoginAdmin", "Auth");

            if (!isAdmin())
                return RedirectToAction("Index", "Author");

            Admin admin = adminHelper.GetById(GetAuthorId());

            if (Hash(PM.OldPassword) == admin.Password && PM.NewPassword == PM.ConfirmPassword)
            {
                admin.Password = Hash(PM.NewPassword);
                adminHelper.Update(admin);

                return RedirectToAction("Profile");
            }
            else
            {
                ModelState.AddModelError("", "Invalid form data!");
                return View(PM);
            }
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