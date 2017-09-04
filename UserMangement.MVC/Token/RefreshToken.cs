using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserMangement.MVC.Controllers
{
    public class RefreshToken
    {
        // HTTPAccessor
        private IHttpContextAccessor _httpContextAccessor;
        HttpClient _httpClient = new HttpClient();

        public RefreshToken(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> RenewToken()
        {

            var currentContext = _httpContextAccessor.HttpContext;

            var discoveryClient = new DiscoveryClient("https://localhost:44391/");
            var metaDataResponse = await discoveryClient.GetAsync();

            // create a new token Client to get new tokens

            var tokenClient = new TokenClient(metaDataResponse.TokenEndpoint,
                "adminMVC", "secrect");


            //get the saved refresh token
            var cuurentRefreshToken =await currentContext.Authentication
                .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);


            // Refresh Token 
            var tokenResult = await tokenClient
                .RequestRefreshTokenAsync(cuurentRefreshToken);

            if (!tokenResult.IsError)
            {
                var authenticateInfo = await currentContext.Authentication
                    .GetAuthenticateInfoAsync("Cookies");

                var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);

                authenticateInfo.Properties.UpdateTokenValue(
                    OpenIdConnectParameterNames.AccessToken, tokenResult.AccessToken);
                authenticateInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken,
                    tokenResult.RefreshToken);

                //Sing Again with new values
                await currentContext.Authentication.SignInAsync("Cookies",
                    authenticateInfo.Principal, authenticateInfo.Properties);
                // return the new access token
                return tokenResult.AccessToken;
            }
            else
            {
                return string.Empty;
                //throw;
                throw new Exception("Errore where refresh token ");
            }
        }
    }
}
