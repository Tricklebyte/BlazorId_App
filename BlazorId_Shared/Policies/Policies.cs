using Microsoft.AspNetCore.Authorization;
using System;

namespace BlazorId_Shared
{
    public class Policies
    {
        public const string CanViewIdentity = "CanViewIdentity";

        public static AuthorizationPolicy CanViewIdentityPolicy()
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("appuser_claim", "identity")
                .Build();
        }

    }
}
