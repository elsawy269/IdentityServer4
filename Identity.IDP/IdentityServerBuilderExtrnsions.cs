using Identity.IDP.Services;
using Marvin.IDP.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.IDP
{
    public static class IdentityServerBuilderExtrnsions
    {
        public static IIdentityServerBuilder AddUserStore(this IIdentityServerBuilder builder)
       
        {
            builder.Services.AddSingleton
                <IUserRepository, UserRepository>();
            builder.AddProfileService<UserProfileService>();
            return builder;
        }
    }
}
