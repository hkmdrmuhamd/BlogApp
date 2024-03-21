using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlogApp.Controllers
{
    public class UsersController : Controller
    {
        public UsersController()
        {
            
        }
        
        public IActionResult Login()
        {
            return View();
        }
    }
}