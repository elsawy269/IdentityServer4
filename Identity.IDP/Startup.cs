using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Marvin.IDP.Entities;
using Marvin.IDP.Services;

namespace Identity.IDP
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<MarvinUserContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddMvc();
            services.AddIdentityServer()
                     .AddTemporarySigningCredential()
                     .AddUserStore()
                     //.AddTestUsers(Config.GetUsers())
                     .AddInMemoryIdentityResources(Config.GetIdentityResources())
                     .AddInMemoryApiResources(Config.GetApiResource())
                     .AddInMemoryClients(Config.GetClients());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            MarvinUserContext marvinUserContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            marvinUserContext.Database.EnsureCreated();
            marvinUserContext.EnsureSeedDataForContext();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

        }
    }
}
