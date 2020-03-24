using DAL.Helpers;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class PictureController : Controller
    {
        private readonly PictureHelper pictureHelper = new PictureHelper();
        private readonly AvatarHelper avatarHelper = new AvatarHelper();

        public FileResult Post(int id)
        {
            Picture picture = pictureHelper.GetById(id);
            if (picture != null && picture.Source?.Length > 0)
                return File(picture.Source, "image/jpeg", picture.PostId + ".jpg");
            return null;
        }

        public FileResult Author(int id)
        {
            Avatar avatar = avatarHelper.GetById(id);
            if (avatar != null && avatar.Source?.Length > 0)
                return File(avatar.Source, "image/jpeg", avatar.AuthorId + ".jpg");
            return null;
        }
    }
}