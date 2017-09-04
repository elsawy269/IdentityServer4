using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UserMangement.MVC.Controllers
{
    public class AuthorizeController : Controller
    {
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}