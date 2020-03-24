using DAL.Helpers;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace MVC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            AdminHelper     Admins      = new AdminHelper();
            
            if(Admins.GetAll().Count() == 0)
            {
                string Hash(string str)
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

                Admin admin = new Admin()
                {
                    DateJoined = DateTime.Now,
                    Email = "admin@gmail.com",
                    FullName = "Default Admin",
                    Password = Hash("admin123")
                };

                Admins.Create(admin);
            }
            
        }
        
    }
}
