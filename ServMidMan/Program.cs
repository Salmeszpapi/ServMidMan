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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Authentication}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
