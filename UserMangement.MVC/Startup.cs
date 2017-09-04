using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using UserMangement.MVC.Controllers;

namespace UserMangement.MVC
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddTransient<RefreshToken>();

            //Add Authorization Policiey 

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    "CardOrderFram",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "ksa");
                        policyBuilder.RequireClaim("subscriptionlevel", "payingUser");
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme="Cookies",
                AccessDeniedPath= "/Authorize/AccessDenied"
            });

            // Clear Claims to reload and get changes 
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                Authority = "https://localhost:44391/",
                RequireHttpsMetadata = true,
                ClientId = "adminMVC",
                Scope = { "openid", "profile","address", "roles" , "UserManagementAPI",
                "country","subscriptionlevel","offline_access"}, //we add scope to ensure it will be include in token
                ResponseType = "code id_token",
                //  CallbackPath=new AsyncCallback("...")
                SignInScheme = "Cookies",
                SaveTokens = true,
                ClientSecret = "secrect",
                GetClaimsFromUserInfoEndpoint = true,

                // Events contain all event you might need when connect with IDP
                Events = new OpenIdConnectEvents()
                {
                    OnTokenValidated = tokenValidatedContexd =>
                    {
                        var identity = tokenValidatedContexd.Ticket.Principal.Identity as ClaimsIdentity;

                        var subject = identity.Claims.FirstOrDefault(z => z.Type == "sub");
                        var newClaimsIdentity = new ClaimsIdentity(
                            tokenValidatedContexd.Ticket.AuthenticationScheme,"given_name","role"
                            );
                        newClaimsIdentity.AddClaim(subject);
                        tokenValidatedContexd.Ticket = new AuthenticationTicket(
                            new ClaimsPrincipal(newClaimsIdentity),
                            tokenValidatedContexd.Ticket.Properties,
                            tokenValidatedContexd.Ticket.AuthenticationScheme);
                        return Task.FromResult(0);
                    },

                    OnUserInformationReceived=userInformationReceivedContext=>
                    {
                        userInformationReceivedContext.User.Remove("address");
                        return Task.FromResult(0);

                    }
                }
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
