// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:44387";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "identityApi";
                });

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:44321")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddAuthorization(authorizationOptions =>
            {
                authorizationOptions.AddPolicy(
                    BlazorId_Shared.Policies.CanViewIdentity,
                    BlazorId_Shared.Policies.CanViewIdentityPolicy());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}