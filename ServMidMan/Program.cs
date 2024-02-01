using Microsoft.EntityFrameworkCore;
using ServMidMan.Data;

namespace ServMidMan
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<DataProviderContext>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnectionString"));
            });
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddSession(options =>
            {
	            options.IdleTimeout = TimeSpan.FromMinutes(30);
	            options.Cookie.HttpOnly = true;
	            options.Cookie.IsEssential = true;
            });
			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Authentication/Login");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthorization();
            app.UseSession();
			app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Authentication}/{action=Welcome}/{id?}");

            app.Run();
        }

    }
}
