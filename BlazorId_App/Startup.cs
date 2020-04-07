using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using BlazorId_App.Data;

namespace BlazorId_App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //create an HTTP client for accessing the API
            services.AddHttpClient("ApiClient", client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = "https://localhost:44387/";
                    options.ClientId = "BlazorID_App";
                    options.ClientSecret = "secret";
                    options.UsePkce = true;
                    options.ResponseType = "code";
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");

                    //Scope for accessing API
                    options.Scope.Add("identityApi"); //invalid scope for client

                    // Scope for custom user claim
                         options.Scope.Add("appUser_claim"); //invalid scope for client

                    // map custom user claim 
                    options.ClaimActions.MapUniqueJsonKey("appUser_claim", "appUser_claim");
                   
                    //options.CallbackPath = ...
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                });

            services.AddAuthorization(authorizationOptions =>
            {
                // add authorization poliy from shared project. This is the same policy used by the API
                authorizationOptions.AddPolicy(
                    BlazorId_Shared.Policies.CanViewIdentity,
                    BlazorId_Shared.Policies.CanViewIdentityPolicy());
            });

            services.AddHttpClient<IdentityDataService, IdentityDataService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5001/");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
