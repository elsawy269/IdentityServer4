using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserMangement.API.Controllers
{
    public class BaseAPIController : Controller
    {
        public string CurrentUser
        {
            get
            {
                return User.Claims.FirstOrDefault(c => c.Type == "sub").Value;
            }
        }
    }
}