using DentLink.DataAccessLayer.Contracts;
using DentLink.DataAccessLayer.Data;
using DentLink.DataAccessLayer.Repositories;
using DentLink.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DentLink.PresentionLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // 🗄️ تسجيل قاعدة البيانات (DbContext)
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // 🚀 [الحل السحري] تسجيل الـ Unit of Work لحل مشكلة الـ Unable to resolve service
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}