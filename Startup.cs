using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Readible.Migrations;
using Readible.Models;
using Readible.Services;
using Readible.Hub;
using Microsoft.AspNetCore.Hosting;

namespace Readible
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up Mvc return Json and ignore null value
            // Comment code in .NET CORE 2.2
            // services.AddMvc().AddJsonOptions(options =>
            //    {
            //        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            //    });

            // Set up Mvc return Json and ignore null value
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>().BuildServiceProvider();

            // Register JWT authentication schema by using AddAuthentication method and specifying JwtBearerDefaults.AuthenticationScheme
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            // Set up CORS for your application add the Microsoft.AspNetCore.Cors package to your project
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", builder => builder.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            });

            // Set up SignalR
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.KeepAliveInterval = TimeSpan.FromMinutes(int.Parse(Configuration["SignalR:IntervalMinutes"]));
            });

            // Set up service for EF Core
            services.AddTransient<BookService>();
            services.AddTransient<BookCategoryService>();
            services.AddTransient<BookCommentService>();
            services.AddTransient<UserService>();
            services.AddTransient<CustomerService>();
            services.AddTransient<ManagerService>();
            services.AddTransient<OrderService>();
            services.AddTransient<MigrationService>();

            // Set up service for SignalR
            services.AddTransient<OrderHub>();
            services.AddTransient<BookCommentHub>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Set up HTTP
            app.UseExceptionHandler("/Error");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Set up CORS for your application add the Microsoft.AspNetCore.Cors package to your project
            app.UseCors("AllowOrigin");

            // Setting up route for SignalR hubs
            app.UseRouting();

            // Add UseAuthentication to make the authentication service available to the application
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderHub>("/hub/orders");
                endpoints.MapHub<CustomerOrderHub>("/hub/orders/customer");
                endpoints.MapHub<BookCommentHub>("/hub/comments");
            });

            app.UseWebSockets();
        }
    }
}