using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserMangement.API.Authorization
{
    public class MustOwnImageRequirements: IAuthorizationRequirement
    {
        public MustOwnImageRequirements()
        {

        }
    }
}
