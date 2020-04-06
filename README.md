# BlazorId_App
Sample Blazor Server Solution with IdentityServer and API
* **Blazor Server Application**
   * Standard Blazor Server template, protected by Identity Server using Authorization Code flow w/PKCE
   * Has protected features to view the User Claim set for the application and api
   * Users of Identity features require custom claim type **appRole_claim** with value **identity**.
* **User Claims API** - returns current user claims as Json 
   * Requires IdentityServer client scope **identityApi**
   * Requires user claim type **appRole_claim** with value  **identity**
   * Authorization policy from BlazorId_Shared is the same one used by the application.
* **IdentityServer Demo** Identity Server samples, quickstart #6 AspNet Identity
  * Standard IdentityServer Demo project - Alice has been granted the **identity** appRole_claim, Bob has not.
