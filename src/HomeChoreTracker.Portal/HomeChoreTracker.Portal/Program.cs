using HomeChoreTracker.Portal.Pages.Home;
using HomeChoreTracker.Portal.ViewComponents;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Security.Claims;

namespace HomeChoreTracker.Portal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            builder.Services.AddScoped<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User);

            builder.Services.AddTransient<HomesListViewComponent>();
            builder.Services.AddTransient<PurchaseHomesListViewComponent>();
            builder.Services.AddTransient<HomeChoreListViewComponent>();
            builder.Services.AddTransient<NotificationListViewComponent>();
			builder.Services.AddTransient<ProfileAvatarViewComponent>();
			builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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

            app.MapRazorPages();

            app.Run();
        }
    }
}
