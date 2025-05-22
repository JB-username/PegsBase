using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PegsBase.Data;
using PegsBase.Models.Constants;
using PegsBase.Models.Identity;
using PegsBase.Models.Settings;
using PegsBase.Models.Subscriptions;
using PegsBase.Services.Emails;
using PegsBase.Services.Identity;
using PegsBase.Services.Parsing;
using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Services.PegCalc.Implementations;
using PegsBase.Services.PegCalc.Interfaces;
using PegsBase.Services.QuickCalcs.Implementations;
using PegsBase.Services.QuickCalcs.Interfaces;
using PegsBase.Services.Settings;
using QuestPDF;
using QuestPDF.Drawing;
using Rotativa.AspNetCore;
using System.Configuration;
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
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            builder.Services.Configure<ClientSettings>(
                builder.Configuration.GetSection("ClientSettings"));

            builder.Services.Configure<SubscriptionApiOptions>(
                builder.Configuration.GetSection("SubscriptionApi"));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
              .AddRoles<IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("SurveyDepartment", policy =>
                    policy.RequireRole(RoleGroups.SurveyDepartment));

                options.AddPolicy("Admins", policy =>
                    policy.RequireRole(RoleGroups.Admins));

                options.AddPolicy("SurveyManagers", policy =>
                    policy.RequireRole(RoleGroups.SurveyManagers));

                options.AddPolicy("MustBeSupervisor", policy =>
                {
                    policy.RequireRole("Supervisor");
                    policy.RequireClaim("CanApprove");
                });
            });

            builder.Services.AddSession();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOpts => npgsqlOpts.UseNetTopologySuite()
                    )
                );

            builder.Services.AddScoped<IPegFileParser, CsvPegFileParser>();
            builder.Services.AddScoped<ICoordinateDatParserService, CoordinateDatParserService>();
            builder.Services.AddScoped<IPegCalcService, PegCalcService>();
            builder.Services.AddScoped<IRawSurveyDataDatFileParser, RawSurveyDataDatFileParser>();
            builder.Services.AddScoped<IMapImportModelsToPegs, MapImportModelsToPegs>();
            builder.Services.AddScoped<IImportSettingsService, ImportSettingsService>();
            builder.Services.AddScoped<IJoinCalculatorService, JoinCalculatorService>();
            builder.Services.AddScoped<ICoordinateConversionService, CoordinateConversionService>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var fontStream = File.OpenRead(
            Path.Combine(
                  builder.Environment.ContentRootPath,
                  "wwwroot",
                  "fonts",
                  "OpenSans.ttf"
                )
            );
            FontManager.RegisterFontWithCustomName("Open Sans", fontStream );
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var app = builder.Build();

            // Configure the HTTP request pipeline. *Middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                RoleSeeder.SeedRolesAsync(services).Wait();
                UserSeeder.SeedUsersAsync(services).Wait();
                MinePlanTypeSeeder.SeedAsync(services).Wait();
            }



            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStaticFiles();


//#if DEBUG
//            RotativaConfiguration.Setup("C:\\Rotativa\\Windows");
//#else
//            RotativaConfiguration.Setup("/usr/local/bin/wkhtmltopdf");
//#endif


            app.UseSession();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
