using IdentityServer4.Services;
using Marvin.IDP.Entities;
using Marvin.IDP.Services;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.IDP.Controllers.UserRegistration
{
    public class UserRegistrationController: Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityServerInteractionService _interaction;

        public UserRegistrationController(IUserRepository userRepository, IIdentityServerInteractionService interaction)
        {
            _userRepository = userRepository;
            _interaction = interaction;
        }

        public IActionResult RegisterUser(string returnUrl)
        {
            return View();
        }


        public async Task<IActionResult> RegisterUserAsync(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usertToCreate = new User();
                usertToCreate.Password = model.Password;
                usertToCreate.Username = model.UserName;
                usertToCreate.IsActive = true;
                usertToCreate.Claims.Add(new UserClaim("country", model.Country));
                usertToCreate.Claims.Add(new UserClaim("address", model.Address));
                usertToCreate.Claims.Add(new UserClaim("given_name", model.FirstName));
                usertToCreate.Claims.Add(new UserClaim("family_name", model.LastName));
                usertToCreate.Claims.Add(new UserClaim("email", model.Email));
                usertToCreate.Claims.Add(new UserClaim("subscriptionlevel", "freeUser"));
                _userRepository.AddUser(usertToCreate);

                if (!_userRepository.Save())
                {
                    return BadRequest();
                }
                await  HttpContext.Authentication.SignInAsync(usertToCreate.SubjectId, usertToCreate.Username);
                //HttpContext.Authentication.SignInAsync(usertToCreate.SubjectId, usertToCreate.Username);
                return Ok();
            }
            return BadRequest();
        }
    }
}
