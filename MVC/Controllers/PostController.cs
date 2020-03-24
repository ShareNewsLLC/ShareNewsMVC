using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using DAL;
using DAL.Helpers;
using DAL.Models;

namespace MVC.Controllers
{
    public class PostController : Controller
    {
        private readonly ContextDB          db                  = new ContextDB();
        private readonly PostHelper         postHelper          = new PostHelper();
        private readonly CategoryHelper     categoryHelper      = new CategoryHelper();
        private readonly PictureHelper      pictureHelper       = new PictureHelper();
        private readonly AuthorHelper       authorHelper        = new AuthorHelper();

        // GET: Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Post post = postHelper.GetById(id.Value);

            if (post == null)
                return HttpNotFound();

            if (!post.isActive)
                if (!LoggedIn())
                    if(!isAuthor() || !isAdmin())
                        return RedirectToAction("Index", "Home");

            Author author = authorHelper.GetById(post.AuthorId);

            ViewBag.isAnonymous = author.isAnonymous;
            ViewBag.AuthorEmail = author.Email;
            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View(post);
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View();
        }

        // POST: Post/Create
        [HttpPost]
        public ActionResult Create(Post post, HttpPostedFileBase imageFile)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            if (ModelState.IsValid)
            {
                post.AuthorId = GetAuthorId();
                post.DateCreated = DateTime.Now;
                post.LastModified = DateTime.Now;
                post.isActive = false;

                db.Posts.Add(post);
                db.SaveChanges();

                // upload image
                CheckAndUploadImage(post.Id, imageFile);

                return RedirectToAction("Index", "Author");
            }

            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View(post);
        }

        // GET: Post/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Post post = postHelper.GetById(id.Value);

            if (post == null)
                return HttpNotFound();

            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View(post);
        }

        // POST: Post/Edit/5
        [HttpPost]
        public ActionResult Edit(Post post, HttpPostedFileBase imageFile)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            if (ModelState.IsValid)
            {
                post.AuthorId = GetAuthorId();
                post.DateCreated = DateTime.Now;
                post.LastModified = DateTime.Now;
                post.isActive = false;

                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();

                // upload image 
                CheckAndUploadImage(post.Id, imageFile);

                return RedirectToAction("Index", "Author");
            }
            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View(post);
        }

        // GET: Post/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Post post = postHelper.GetById(id.Value);

            if (post == null)
                return HttpNotFound();

            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive);
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!LoggedIn())
                return RedirectToAction("Login", "Auth");

            if (!isAuthor() && !isAdmin())
                return RedirectToAction("Logout", "Auth");

            if (isAdmin())
                return RedirectToAction("Index", "Admin");

            if (!authorHelper.GetById(GetAuthorId()).isVerified)
                return RedirectToAction("Wait", "Author");

            postHelper.Delete(id);
            try
            {
                pictureHelper.Delete(pictureHelper.GetById(id).Id);
            }
            catch (Exception) { }

            return RedirectToAction("Index", "Author");
        }

        private void CheckAndUploadImage(int PostId, HttpPostedFileBase imageFile)
        {

            Picture lastPic = pictureHelper.GetAll().SingleOrDefault(p => p.PostId == PostId);

            if (imageFile?.ContentLength > 0 && imageFile.ContentType.Contains("image"))
            {
                using (var stream = new MemoryStream())
                {
                    imageFile.InputStream.CopyTo(stream);
                    if (lastPic != null)
                    {
                        lastPic.Source = stream.ToArray();
                        pictureHelper.Update(lastPic);
                    }
                    else
                    {
                        lastPic = new Picture() { PostId = PostId, Source = stream.ToArray() };
                        pictureHelper.Create(lastPic);
                    }

                }
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
