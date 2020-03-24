using DAL.Helpers;
using DAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly PostHelper         postHelper      = new PostHelper();
        private readonly CategoryHelper     categoryHelper  = new CategoryHelper();
        private readonly AuthorHelper       authorHelper    = new AuthorHelper();


        public ActionResult Index(string currentFilter, string searchString, int? page)
        {

            ViewBag.Categories = categoryHelper.GetAll().Where(cat => cat.isActive);
            

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var posts = from p in postHelper.GetAll().Where(post => post.isActive).OrderByDescending(p => p.DateCreated).ToList() select p;

            if (!string.IsNullOrEmpty(searchString))
                posts = posts.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));


            int pageSize = 16;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Category(int id, string currentFilter, string searchString, int? page)
        {
            Category category = categoryHelper.GetById(id);

            if (!category.isActive)
                return RedirectToAction("Index");

            if (category == null)
                return RedirectToAction("Index");
            
            ViewBag.SelectedCategory = category;

            ViewBag.Categories = categoryHelper.GetAll().Where(cat => cat.isActive);


            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var posts = from p in postHelper.GetAll().Where(post => post.CategoryId == id && post.isActive).OrderByDescending(p => p.DateCreated).ToList() select p;

            if (!string.IsNullOrEmpty(searchString))
                posts = posts.Where(p => p.Title.ToLower().Contains(searchString.ToLower()));


            int pageSize = 16;
            int pageNumber = (page ?? 1);
            return View(posts.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Author(int id)
        {
            Author author = authorHelper.GetById(id);

            author.Password = "";

            ViewBag.Author = author;
            ViewBag.Categories = categoryHelper.GetAll().Where(c => c.isActive).ToList();

            if (!author.isAnonymous)
                return View(postHelper.GetAll().Where(p => p.AuthorId == author.Id && p.isActive).ToList());
            
            if((LoggedIn() && isAdmin()) || (LoggedIn() && GetAuthorId() == id))
                return View(postHelper.GetAll().Where(p => p.AuthorId == author.Id).ToList());

            return RedirectToAction("Index");
        }

        public ActionResult TOC()
        {
            return View();
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