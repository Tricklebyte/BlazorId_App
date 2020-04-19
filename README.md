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
Navigation Menu Item: calls a test API, which is protected by IdentityServer. The API will return the user claims it received with the request as JSON to the application. The application then display those claims to the User. 
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
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
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

 ## Configuration
### Custom User Claim - appUser_Claim
#### Application user claim
   * Required configuration for the custom user claim to be added to the application cookie during IdentityServer login:
   1. The claim must be present for the user in the user store.
   2. A custom Identity Resource must configured in Identity Server for the custom claim:
         ```c#
           new IdentityResource("appUser_claim", new []{"appUser_claim"})
         ```
   3. The client config in IdentityServer must include this identity resource in the client's **'Allowed Scopes'**:
         ```c#
            client.AllowedScopes = {"openid","profile","email","identityApi","appUser_Claim"};
         ```
   4. The client must request this scope in the Oidc configuration settings in Startup.ConfigureServices:
         ```c#
             options.Scope.Add("appUser_claim"); 
         ```
   5. The client must map the IdentityServer Claim type to the Blazor App Claim type in the Oidc configuration settings in Startup.ConfigureServices:
      ```c#
       options.ClaimActions.MapUniqueJsonKey("appUser_claim", "appUser_claim");
      ```
 #### API user claim
 *Additional configuration requirements for the custom user claim to be included in the Access token for the API
   1.  The API Resource configuration in IdentityServer must include the custom claim type in the api resource's **ClaimTypes**. The following code creates the API resource and assigns the claim type of **appUser_claim**:
 ```c#
   new ApiResource("identityApi", 
                   "Identity Claims Api", 
                    new []{"appUser_claim"});

 ```
