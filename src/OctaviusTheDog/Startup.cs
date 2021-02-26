using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OctaviusTheDog.DataAccess.AzureCosmos;
using OctaviusTheDog.DataAccess.AzureStorage;
using OctaviusTheDog.DataAccess.SendGrid;
using System;

namespace OctaviusTheDog
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IAzureCosmosRepository, AzureCosmosRepository>(c => new AzureCosmosRepository(GetConnectionString("CosmosDB")))
                    .AddScoped<IAzureStorageRepository, AzureStorageRepository>(c => new AzureStorageRepository(GetConnectionString("StorageAccount")))
                    .AddScoped<IMailSender, MailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private string GetConnectionString(string name)
        {
            if (_webHostEnvironment.IsProduction())
            {
                return Environment.GetEnvironmentVariable($"CUSTOMCONNSTR_{name}");
            }

            return _configuration.GetConnectionString(name);
        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
    }
}
