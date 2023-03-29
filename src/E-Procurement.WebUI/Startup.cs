using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using E_Procurement.Data;
using E_Procurement.Data.Entity;
using E_Procurement.Repository.AutofacModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using E_Procurement.WebUI.Models.Service;
using E_Procurement.Repository.Utility.Models;
using E_Procurement.Repository.Interface;
using E_Procurement.Repository.Utility;
using System.IO;
using DinkToPdf.Contracts;
using DinkToPdf;
using E_Procurement.WebUI.Service;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace E_Procurement.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IHostingEnvironment HostingEnvironment { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //});
            

            services.AddDbContext<EProcurementContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<User>()
                .AddRoles<Role>()
                .AddUserManager<UserManager<User>>()
                .AddRoleManager<RoleManager<Role>>()
                .AddSignInManager<SignInManager<User>>()
                .AddEntityFrameworkStores<EProcurementContext>();
                

            //services.AddIdentity<User, Role>()
            //   .AddUserManager<UserManager<User>>()
            //   .AddSignInManager<SignInManager<User>>()
            //   .AddRoleManager<RoleManager<Role>>()
            //   .AddEntityFrameworkStores<EProcurementContext>()
            //    .AddDefaultTokenProviders();




            services.AddAutoMapper(typeof(Startup).Assembly);

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            //services.AddSingleton<ISMTPService, SMTPService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            


            // for PDF conversion
            var wkHtmlToPdfPath = Path.Combine(HostingEnvironment.ContentRootPath, $"libwkhtmltox");

            CustomAssemblyLoadContext ctext = new CustomAssemblyLoadContext();
            ctext.LoadUnmanagedLibrary(wkHtmlToPdfPath);
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

            var builder = new ContainerBuilder();
            builder.Populate(services);

           // builder.RegisterType<EmailSettings>().AsSelf().SingleInstance();
            builder.RegisterModule<AutofacRepoModule>();

            ApplicationContainer = builder.Build();
            // Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(ApplicationContainer);


         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
         

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();        

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
               
            });

            app.UseCookiePolicy();
           CreateUserRoles(services).Wait();
            appLifetime.ApplicationStopped.Register(() => ApplicationContainer.Dispose());
        
        }
        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var user = new User() { Email = "admin@gmail.com", UserName = "admin" };
            var result = await UserManager.CreateAsync(user, "Pa$$word123");

            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                //create the roles and seed them to the database  
                //await RoleManager.CreateAsync(new Role("Admin"));
                await RoleManager.CreateAsync(new Role() { Name = "Admin"});
            }
            await UserManager.AddToRoleAsync(user, "Admin");

        }
    }
}
