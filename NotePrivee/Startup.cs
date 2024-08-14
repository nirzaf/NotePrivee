using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.EntityFrameworkCore;
using Westwind.AspNetCore.Markdown;
using Microsoft.OpenApi.Models;

namespace NotePrivee
{
    public class Startup
    {
        public IWebHostEnvironment CurrentEnvironment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("NoteBDD");
            services.AddMvc();

            if (CurrentEnvironment.IsEnvironment("Testing"))
            {
                services.AddDbContext<notepriveeContext>(options => options.UseInMemoryDatabase("FakeDB"));
            }
            else
            {
                services.AddDbContext<notepriveeContext>(options => options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31))));
                services.AddMarkdown();
            }

            services.AddControllersWithViews();
            services.AddMvcCore().AddApiExplorer();
            services.AddAuthentication("CookieAuthentication")
                 .AddCookie("CookieAuthentication", config =>
                 {
                     config.Cookie.Name = "UserLoginCookie";
                     config.LoginPath = "/Admin/Login";
                 });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Note Privée API",
                    Version = "v1"
                });
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (!env.IsEnvironment("Testing"))
            {
                app.UseMarkdown();
            }

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Notes}/{action=Index}/{id?}");
            }
            );

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Note Privée");
            });

        }
    }
}
