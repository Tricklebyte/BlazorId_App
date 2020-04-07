// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServerAspNetIdentity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                // Identity Resource for custom user claim type
                new IdentityResource("appUser_claim", new []{"appUser_claim"})
            };


        public static IEnumerable<ApiResource> Apis =>
            new List<ApiResource>
            {   // Identity API, consumes user claim type 'appUser_claim'
                // By assigning the user claim to the api resource, we are instructing Identity Server to include that claim in Access tokens for this resource.
                new ApiResource("identityApi", 
                                "Identity Claims Api", 
                                 new []{"appUser_claim"})
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // machine to machine client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // scopes that client has access to
                    AllowedScopes = { "identityApi" },
                },
                // interactive ASP.NET Core MVC client
                new Client
                {
                    ClientId = "BlazorID_App",
                    ClientName="Blazor Server App - Identity Claims",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,
                
                    // where to redirect to after login
                    RedirectUris = { "https://localhost:44321/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:44321/signout-callback-oidc" },

                    AllowedScopes = { "openid", "profile", "email", "identityApi","appUser_claim" },


                      
             
                   AllowOfflineAccess = true
                }
            };
    
    
    }



}