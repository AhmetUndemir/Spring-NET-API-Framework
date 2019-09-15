using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimpleSprint.Business;
using SimpleSprint.Business.Interfaces;
using SimpleSprint.Data;
using SimpleSprint.Data.Interfaces;
using SimpleSprint.Middleware;
using SimpleSprint.Models.Auth;
using SimpleSprint.Repos;
using SimpleSprint.Repos.Interfaces;

namespace SimpleSprint {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            services.AddDbContext<SsDbContext> (opts => opts.UseSqlServer (Configuration["ConnectionString:SimpleSprintDB"]));
            services.AddScoped<IUnitOfWork, UnitOfWork> ();
            services.AddScoped<IAuthRepository, AuthRepository> ();
            services.AddScoped<IAuthBusiness, AuthBusiness> ();

            IdentityBuilder builder = services.AddIdentityCore<User> (opt => {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredUniqueChars = 0;
                opt.Password.RequireLowercase = false;
            });

            builder = new IdentityBuilder (builder.UserType, typeof (Role), builder.Services);
            builder.AddEntityFrameworkStores<SsDbContext> ();
            builder.AddRoleValidator<RoleValidator<Role>> ();
            builder.AddRoleManager<RoleManager<Role>> ();
            builder.AddSignInManager<SignInManager<User>> ();

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_2);
            var key = Encoding.ASCII.GetBytes (Configuration["Application:Secret"]);
            services.AddAuthentication (x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer (options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                new SymmetricSecurityKey (key),
                ValidateIssuer = false,
                ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts ();
            }

            app.UseMiddleware<ApiResponseMiddleware> ();

            app.UseAuthentication ();
            // app.UseHttpsRedirection ();
            app.UseMvc (routes => {
                routes
                    .MapRoute (name: "default", template: "{controller=Home}/{action=Index}/{id?}")
                    .MapRoute (name: "api", template: "api/{controller}/{action}/{id?}");
            });
        }
    }
}