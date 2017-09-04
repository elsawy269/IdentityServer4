using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using IdentityModel.Client;
using UserMangement.MVC.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace UserMangement.MVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        // HTTPAccessor
        private IHttpContextAccessor _httpContextAccessor;
        HttpClient _httpClient = new HttpClient();
        private RefreshToken _RefreshToken;
        public HomeController(IHttpContextAccessor httpContextAccessor, RefreshToken RefreshToken)
        {
            _httpContextAccessor = httpContextAccessor;
            _RefreshToken = RefreshToken;
        }
        public async Task<IActionResult> Index()
        {
            await GetOutIdentityTokenAsync();
            //return View();
            return   await CallUMAPI();
        }
        public async Task GetOutIdentityTokenAsync()
        {
            var idToken = await HttpContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            foreach (var item in User.Claims)
            {
                // Name PAssing must match name passing when configure Cookie authentication
               // await HttpContext.Authentication.SignOutAsync("Cookies");
            }
        }
        public async Task Logout()
        {

            // Name PAssing must match name passing when configure Cookie authentication
            await HttpContext.Authentication.SignOutAsync("Cookies");
            // Name PAssing must match name passing when configure  OpenId MiddleWare

            await HttpContext.Authentication.SignOutAsync("oidc");

        }
        [Authorize(Roles = "payingUser")]
        public async Task<IActionResult> OrderFram()
        {
            var discoveryClient = new DiscoveryClient("https://localhost:44391/");
            var metaDataResponse = await discoveryClient.GetAsync();

            var userInfoClient = new UserInfoClient(metaDataResponse.UserInfoEndpoint);

            var accessToken = await HttpContext.Authentication
                .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await userInfoClient.GetAsync(accessToken);

            if (response.IsError)
            {
                throw new Exception();
            }
            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            return View(new OrderFramViewModel(address));
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> CallUMAPI()
        {
             
            string accessToken = string.Empty;
            var currentContext = _httpContextAccessor.HttpContext;
            //  accessToken = await currentContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var expires_at = await currentContext.Authentication.GetTokenAsync("expires_at");

            if (string.IsNullOrEmpty(expires_at)||
                (DateTime.Parse(expires_at).AddSeconds(-60).ToUniversalTime()<DateTime.UtcNow))
            {
                accessToken = await _RefreshToken.RenewToken();
            }
            else
            {
                accessToken = await currentContext.Authentication.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }
            if (!String.IsNullOrWhiteSpace(accessToken))
            {
                _httpClient.SetBearerToken(accessToken);
            }


            _httpClient.BaseAddress = new Uri("https://localhost:44382/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var res = await _httpClient.GetAsync("api/values");

            if ( res.IsSuccessStatusCode)
            {

            }
            else if(res.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                    res.StatusCode== System.Net.HttpStatusCode.Forbidden)
            {
                return RedirectToAction("AccessDenied", "Authorize");
            }
            return View();
        }
    }
}
