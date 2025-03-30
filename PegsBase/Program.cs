using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PegsBase.Data;
using PegsBase.Services.Parsing;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Implementations;
using PegsBase.Services.PegCalc.Interfaces;
using System.Data.Common;
using System.Globalization;

namespace PegsBase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container. *Dependency Injection
            builder.Services.AddSession();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            builder.Services.AddScoped<IPegFileParser, CsvPegFileParser>();
            builder.Services.AddScoped<ICoordinateDatParserService, CoordinateDatParserService>();
            builder.Services.AddScoped<IPegCalcService, PegCalcService>();
            builder.Services.AddScoped<IRawSurveyDataDatFileParser, RawSurveyDataDatFileParser>();


            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;


            var app = builder.Build();

            // Configure the HTTP request pipeline. *Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStaticFiles();
            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
