# BlazorId_App
Sample Blazor Server Solution with IdentityServer and API
* **Blazor Server Application**
   * Standard Blazor Server template, protected by Identity Server using Authorization Code flow w/PKCE
   * Has protected views that display the User Claim set for the application or api
      * Identity features protected by Authorization policy **CanViewIdentity** from BlazorId_Shared. The policy requires custom claim type **appRole_claim** with value **identity**.
      * **CascadingAuthenticationState** is declared in file App.razor to support UI authorization functions
      * **AuthorizeView** component is used in file NavMenu.razor to show / hide menu options based on authorization state
* **User Claims API** - returns current user claims as Json 
   * Requires IdentityServer client scope **identityApi**
   * Requires user claim type **appRole_claim** with value  **identity**
   * Authorization policy from BlazorId_Shared is the same one used by the application.
* **IdentityServer Demo** Identity Server samples, quickstart #6 AspNet Identity
  * Standard IdentityServer Demo project - Alice has been granted the **identity** appRole_claim, Bob has not.
  
