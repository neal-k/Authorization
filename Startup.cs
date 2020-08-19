using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthorizationRequirements;
using Microsoft.AspNetCore.Mvc.Authorization;
using Transformer;
using Microsoft.AspNetCore.Authentication;
using CustomPolicyProvider;
using static CustomPolicyProvider.CustomAuthorizationPolicyProvider;

namespace authentication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config => 
                {
                    config.Cookie.Name = "Grandmas.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            // MVC also allows custom authorization filters which will be global
            services.AddControllersWithViews(config => 
            {
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                    .RequireAuthenticatedUser()
                    .Build();

                // global authorization filter
                // config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });

            services.AddAuthorization(config => 
            {
                // <defaultAuth> how default auth is configured
                // var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                // var defaultAuthPolicy = defaultAuthBuilder
                //     .RequireAuthenticatedUser()
                //     .RequireClaim(ClaimTypes.DateOfBirth)
                //     .Build();
                
                // config.DefaultPolicy = defaultAuthPolicy;
                // </defaultAuth>

                config.AddPolicy("Claim.DoB", policyBuilder => 
                {
                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });

                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

            });

            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // who are you?
            app.UseAuthentication();

            // are you allowed? 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
