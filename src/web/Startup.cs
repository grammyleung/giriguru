using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GiriGuru.Web.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GiriGuru.Web
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
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => false;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});




			/* Start Adding based on default setting */

			// localization
			services.AddLocalization(options => options.ResourcesPath = "Resources");
			// cache
			services.AddDistributedMemoryCache();
			// session
			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(60);
				options.Cookie.HttpOnly = true;
			});

			//Cross-Origin Resource Sharing
			services.AddCors();

			/* End Adding based on default setting */




			// MVC
			services.AddMvc()
				.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
				.AddDataAnnotationsLocalization()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);




			/* Start Localization Setting */
			services.Configure<RequestLocalizationOptions>(options =>
			{
				var supportedCultures = new List<CultureInfo>
				{
					new CultureInfo("en"),
					new CultureInfo("zh")
				};
				options.DefaultRequestCulture = new RequestCulture("en");
				options.SupportedCultures = supportedCultures;
				options.SupportedUICultures = supportedCultures;
			});
			/* End Localization Setting */




			/* Start Db Settings Related */
			// configure strongly typed settings objects
			var dbSettingsSection = Configuration.GetSection("DbSettings");
			services.Configure<DbSettings>(dbSettingsSection);
			AppSettings.DbSettings = dbSettingsSection.Get<DbSettings>();
			/* End Db Settings Related */


			/* Start File Settings Related */
			// configure strongly typed settings objects
			var fileSettingsSection = Configuration.GetSection("FileSettings");
			services.Configure<FileSettings>(fileSettingsSection);
			AppSettings.FileSettings = fileSettingsSection.Get<FileSettings>();
			/* End File Settings Related */


			/* Start User Settings Related */
			// configure strongly typed settings objects
			var adminSettingsSection = Configuration.GetSection("AdminSettings");
			services.Configure<AdminSettings>(adminSettingsSection);
			AppSettings.AdminSettings = adminSettingsSection.Get<AdminSettings>();
			/* End User Settings Related */
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseCookiePolicy();

			/* Start Adding based on default setting */
			app.UseHttpsRedirection();
			app.UseSession();

			// global cors policy
			app.UseCors(x => x
				.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials()
			);

			// localization
			var supportedCultures = new[]
			{
				new CultureInfo("en"),
				new CultureInfo("zh")
			};
			app.UseRequestLocalization(new RequestLocalizationOptions
			{
				DefaultRequestCulture = new RequestCulture("en"),
				// Formatting numbers, dates, etc.
				SupportedCultures = supportedCultures,
				// UI strings that we have localized.
				SupportedUICultures = supportedCultures
			});
			/* End Adding based on default setting */

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
