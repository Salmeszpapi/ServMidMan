using Microsoft.EntityFrameworkCore;
using ServMidMan.Data;
using ServMidMan.Hubs;
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
            builder.Services.AddTransient<ChatHub>();
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();
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
                app.UseExceptionHandler("/Home/Index");
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
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<ChatHub>("/chatHub");
            app.Run();
        }

    }
}
