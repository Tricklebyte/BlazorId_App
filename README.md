# BlazorId_App
**Sample Blazor Server Application (with IdentityServer and API)<br/>**
This Example solution demonstrates how to:
* Integrate a Blazor Server application with IdentityServer and ASP.NET Identity using auth code flow with PKCE protection.
* Configure a custom user claim in Identity Server and propagate it to the application cookie during user authentication.
* Include the custom user claim in the access token when calling the API.
* Use a shared authorization policy to secure the application and API.

# Blazor App Features
This application provides two protected User features that allow the user to view all claims that have been assigned, and to differentiate between the Application user claims set and the API user claims set.
### APP Identity 
Navigation Menu Item: displays the claims of the current User identity for the application.<br/> 
### API Identity 
Navigation Menu Item: calls a test API, which is protected by IdentityServer. The API will return the user claims it received with the request as JSON to the application. The application then displays those claims to the User. 
### Authorization
* Navigation menu items appear in the Left Nav Menu only when a User is authorized to use them. 
* These app features, and the API controller are protected by a single authorization policy located in a shared project.
* The policy requires custom user claim **userApp_claim** with value **identity**.

# Step 1 IdentityServer Configuration

## User with custom claim
Uses IdentityServer Quickstart 6 AspNetIdentity sample project.<br/>
Test users and claims are created in SeedData.cs. <br/>
Alice is assigned custom claim type userApp_claim with value **identity**<br/><br/>
**SeedData.cs**<br/>
```c#
result = userMgr.AddClaimsAsync(alice, new Claim[]{
             new Claim(JwtClaimTypes.GivenName, "Alice"),
             new Claim(JwtClaimTypes.FamilyName, "Smith"),
             new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
             new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
             new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
             new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
             // Add user_scope claim for Identity to authorize UI and API actions. Alice has this claim, Bob does not.
             new Claim("appUser_claim","identity")

```

## Identity Resource
A custom Identity Resource is required in IdentityServer to control access to the custom claim type **appRole_claim**  for client applications and apis.<br/><br/>
**Config.cs**<br/>
```c#
new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // Identity Resource for custom user claim type
                new IdentityResource("appUser_claim", new []{"appUser_claim"})
            };
```

## Api Resource
A custom API Resource is required in IdentityServer to control access to the API and specify which user claims should be included in the Access token.
<br/><br/>
**Config.cs**<br/>
```c#
   new List<ApiResource>
            {   // Identity API, consumes user claim type 'appUser_claim'
                    // Claim Types are the associated user claim types required by this resource (api).
                    // Identity Server will include those claims in Access tokens for this resource when available.
                new ApiResource("identityApi",              // Name
                                "Identity Claims Api",      // Display Name
                                 new []{"appUser_claim"})   // Claim Types
            };
```

## Client
A client must be configured in Identity Server that has access to the API Resource and the Identity Resource.<br/><br/>
**Config.cs**<br/>
```c#
 // interactive ASP.NET Core Blazor Server Client
       new Client
           {
               ClientId = "BlazorID_App",
               ClientName="Blazor Server App - Identity Claims",
               ClientSecrets = { new Secret("secret".Sha256()) },

               // Use Code flow with PKCE (most secure)
               AllowedGrantTypes = GrantTypes.Code,
               RequirePkce = true,
                    
               // Do not require the user to give consent
               RequireConsent = false,                   
                
               // where to redirect to after login
               RedirectUris = { "https://localhost:44321/signin-oidc" },

               // where to redirect to after logout
               PostLogoutRedirectUris = { "https://localhost:44321/signout-callback-oidc" },

               // Allowed Scopes - include Api Resources and Identity Resources that may be accessed by this client
               // The identityApi scope provides access to the API, the appUser_claim scope provides access to the custom Identity Resource
               AllowedScopes = { "openid", "profile", "email", "identityApi","appUser_claim" },

               // AllowOfflineAccess includes the refresh token
               // The application will get a new access token after the old one expires without forcing the user to sign in again.
               // Token management is done by the middleware, but the client must be allowed access here and the offline_access scope must be added in the OIDC settings in client Startup.ConfigureServices
               AllowOfflineAccess = true
           }
 ```
 
 # Step 2 Configure the API
 The demo API was created from the standard ASP.NET Core Web API template.
 ## IdentityController
 Add a new Controller to the project named IdentityController with the following code:
 ```c#
  //create base controller route
    [Route("api/identity")]

    // This authorize attribute challenges all clients attempting to access all controller methods.
    // Clients must posses the client scope claim "identityApi" (api resource in IdentityServer)
    // It is not actually required in this specific case, because there is only one method and it has its own Authorize attribute.
    // However, it is a common practice to have this controller level attribute to ensure that Identity Server is protecting the entire controller, including methods that may be added in the future.
    [Authorize]

    public class IdentityController : ControllerBase
    {
        [HttpGet]
        // Use samed shared authorization policy to protect the api GET method that is used to protect the application feature
        // This checks for the user claim type appRole_Claim with value "identity".
        [Authorize(Policy = Policies.CanViewIdentity)]
        public IActionResult Get()
        {
            // return the claim set of the current API user as Json
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
 
 ```
 
 
 ## Startup.ConfigureServices
 ### Authentication
 ### Authorization
 ## Startup.Configure
