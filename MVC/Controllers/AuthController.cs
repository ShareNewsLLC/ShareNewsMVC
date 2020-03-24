using DAL.Helpers;
using DAL.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MVC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly AdminHelper adminHelper = new AdminHelper();
        private readonly AuthorHelper authorHelper = new AuthorHelper();
        private readonly AvatarHelper avatarHelper = new AvatarHelper();
        private readonly EmailHelper emailHelper = new EmailHelper();

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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
            /* ModelState.IsValid && ValidateCaptcha() */
            if (ValidateCaptcha())
            {
                string hashedPassword = Hash(loginModel.Password);
                Author author = authorHelper.GetAll().SingleOrDefault(a => a.Email == loginModel.Email.ToLower() && a.Password == hashedPassword);


                if (author != null)
                {
                    var ident = new ClaimsIdentity(new[]{
                        new Claim(ClaimTypes.Name, author.FullName),
                        new Claim(ClaimTypes.NameIdentifier, author.Id.ToString()),
                        new Claim(ClaimTypes.Role, "Author"),
                        new Claim(ClaimTypes.Email, $"{author.Email}"),
                     }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties{
                        IsPersistent = loginModel.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    }, ident);

                    return RedirectToAction("Index", "Author");
                }
                
                ModelState.AddModelError("", "Login and password did not match!");
                return View(loginModel);
            }
            ModelState.AddModelError("", "Invalid Input! Validate the captcha");
            return View(loginModel);
        }

        public ActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdmin(LoginModel loginModel)
        {
            /* ModelState.IsValid && ValidateCaptcha() */
            if (ValidateCaptcha())
            {
                string hashedPassword = Hash(loginModel.Password);
                Admin admin = adminHelper.GetAll().SingleOrDefault(a => a.Email == loginModel.Email.ToLower() && a.Password == hashedPassword);


                if (admin != null)
                {
                    var ident = new ClaimsIdentity(new[]{
                        new Claim(ClaimTypes.Name, admin.FullName),
                        new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                        new Claim(ClaimTypes.Role, "Admin"),
                        new Claim(ClaimTypes.Email, $"{admin.Email}"),
                    }, DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = loginModel.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    }, ident);

                    return RedirectToAction("Index", "Admin");
                }

                ModelState.AddModelError("", "Login and password did not match!");
                return View(loginModel);
            }
            ModelState.AddModelError("", "Invalid Input! Validate the captcha");
            return View(loginModel);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(Author author, HttpPostedFileBase imageFile)
        {
            // send email to confirm
            // ModelState.IsValid && ValidateCaptcha()
            if (ValidateCaptcha())
            {
                try
                {
                    // set initial values for date joined, is email confirmed, is verified
                    author.DateJoined = DateTime.Now;
                    author.isEmailConfirmed = false;
                    author.isVerified = false;

                    author.Email = author.Email.ToLower();
                    // hash the password here
                    author.Password = Hash(author.Password);

                    authorHelper.Create(author);

                    Author added = authorHelper.GetAll().SingleOrDefault(a => a.Email == author.Email);

                    if (added != null)
                    {
                        // send email confirmation code
                        emailHelper.SendCode(added.Id);

                        // set profile picture
                        Avatar avatar = avatarHelper.GetAll().SingleOrDefault(ava => ava.AuthorId == added.Id);

                        if (imageFile?.ContentLength > 0 && imageFile.ContentType.Contains("image"))
                        {
                            using (var stream = new MemoryStream())
                            {
                                imageFile.InputStream.CopyTo(stream);
                                if(avatar != null)
                                {
                                    avatar.Source = stream.ToArray();
                                    avatarHelper.Update(avatar);
                                }
                                else
                                {
                                    avatar = new Avatar() { AuthorId = added.Id, Source = stream.ToArray() };
                                    avatarHelper.Create(avatar);
                                }
                                
                            }
                        }
                    }
                    
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View();
                }
            }
            ModelState.AddModelError("", "Invalid Input! Validate the captcha");
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Login");
        }

        private bool ValidateCaptcha()
        {
            var client = new WebClient();
            var reply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret=6LfEveMUAAAAAFkEFfQTrO5-1XT5QDpRQLcZ7u9A&response={0}", Request["g-recaptcha-response"]));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes.Count() <= 0) return false;

                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ModelState.AddModelError("", "The secret parameter is missing.");
                        break;
                    case ("invalid-input-secret"):
                        ModelState.AddModelError("", "The secret parameter is invalid or malformed.");
                        break;
                    case ("missing-input-response"):
                        ModelState.AddModelError("", "The response parameter is missing. Please, preceed with reCAPTCHA.");
                        break;
                    case ("invalid-input-response"):
                        ModelState.AddModelError("", "The response parameter is invalid or malformed.");
                        break;
                    default:
                        ModelState.AddModelError("", "Error occured. Please try again");
                        break;
                }
                return false;
            }

            return true;
        }

        private class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

    }
}