oAuth 2 + OpenId




Secure Api 

1. Install access token validation packages
     IdentityServer4.AccessTokenValidation
2. Add middleware to request pipeline at api  before mvc middleware


           app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions {
              Authority= "https://localhost:44391/", //IDP URL
               ApiName= "UserManagementAPI",
               RequireHttpsMetadata=true
           });
Authorization Policies and Access Control

           //Add Authorization Policiey 

           services.AddAuthorization(authorizationOptions =>
           {
               authorizationOptions.AddPolicy(
                   "OrderFram",
                   policyBuilder =>
                   {
                       policyBuilder.RequireAuthenticatedUser();
                       policyBuilder.RequireClaim("country", "ksa");
                       policyBuilder.RequireClaim("subscriptionlevel", "payingUser");
                   });
           });
Install Microsoft.AspNetCore.Authorization;

Using Microsoft.AspNetCore.Authorization;
@inject IAuthorizationService AuthorizationService  --> at view




Creating Custom Requirements and Handlers

           services.AddAuthorization(authorizationOptions =>
           {
               authorizationOptions.AddPolicy(
                   "MustOwnImage",
                   policyBuilder =>
                   {
                       policyBuilder.RequireAuthenticatedUser();
                       policyBuilder.AddRequirements(new MustOwnImageRequirements());
                   });
           });

           services.AddSingleton<IAuthorizationHandler, MustOwnImageHandler>();
+
Using 
       [Authorize("MustOwnImage")]

Refresh Token If token experied
Demo - Creating a Custom User Store



