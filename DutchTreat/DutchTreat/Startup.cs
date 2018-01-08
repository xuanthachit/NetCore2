using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DutchTreat.Data;
using DutchTreat.Data.Entities;
using DutchTreat.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DutchTreat
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment env;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this._configuration = configuration;
            this.env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add Identity
            services.AddIdentity<StoreUsers, IdentityRole>( cfg =>
            {
                cfg.User.RequireUniqueEmail = true;
                //cfg.Password.RequireDigit = true;
            })
            .AddEntityFrameworkStores<DutchContext>();

            // Support tokens as well as the cookies
            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(
                    // Tell Startup about our token
                    cfg => {
                        cfg.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = _configuration["Tokens:Issuer"],
                            ValidAudience = _configuration["Tokens:Audience"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]))
                        };
                    }
                ); 


            services.AddDbContext<DutchContext>(cfg => {
                cfg.UseSqlServer(_configuration.GetConnectionString("DutchConnectionString"));
            });

            // Auto mapper
            services.AddAutoMapper();

            // Support for real mail service
            services.AddTransient<IMailService, NullMailService>();
            services.AddTransient<DutchSeeder>();
            // Register repository
            services.AddScoped<IDutchRepository, DutchRepository>();

            services.AddMvc(opt => {
                if (this.env.IsProduction() && _configuration["DisableSSL"] != "true")
                {
                    opt.Filters.Add(new RequireHttpsAttribute());
                }
            })
            .AddJsonOptions(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore ); // Support return Json result
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("<html><body><h1>Hello World! </h1></body></html>");
            //});


            // set index.html to default 
            //app.UseDefaultFiles();

            // Serving File
            app.UseStaticFiles();

            // Enable Authentication: is the pipline, so you really need authentication before MVC
            app.UseAuthentication();

            app.UseMvc(cfg =>
            {
                cfg.MapRoute("Default", 
                    "{controller}/{action}/{id?}",
                    new { controller = "App", action = "Index"
                    });
            });

            if (env.IsDevelopment())
            {
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
                    seeder.Seed().Wait();
                }
            }
        }
    }
}
