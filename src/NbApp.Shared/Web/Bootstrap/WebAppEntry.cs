using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using NbApp.Web.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace NbApp.Web.Bootstrap
{
    public static class WebAppEntry
    {
        public static async Task Run(string[] args)
        {
            if (args.Length > 0)
            {
                Console.WriteLine("==== args ====");
                foreach (var item in args)
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine();
            }

            var builder = WebApplication.CreateBuilder(args);

            SetupAppService.AddTheAppService(builder.Services);

            builder.Services
                .AddRazorPages(options => {
                    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                })
                .AddRazorRuntimeCompilation();

            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var appSetting = scope.ServiceProvider.GetService<IOptions<TheAppSetting>>().Value;
                if (!string.IsNullOrWhiteSpace(appSetting.PathBase) || appSetting.PathBase != "/")
                {
                    app.UsePathBase(appSetting.PathBase);
                }
            }

            app.UseDeveloperExceptionPage();
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseStaticFiles();
            RuntimeStaticFilesSetup.UseRuntimeStatics(app);
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();

            await app.RunAsync();
        }
    }
}