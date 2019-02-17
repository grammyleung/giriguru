using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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

			if (AppSettings.DbSettings != null)
			{
				if (!string.IsNullOrEmpty(AppSettings.DbSettings.DatabaseName)) Virgo.DB.Config.DatabaseName = AppSettings.DbSettings.DatabaseName;
				if (!string.IsNullOrEmpty(AppSettings.DbSettings.Username)) Virgo.DB.Config.Username = AppSettings.DbSettings.Username;
				if (!string.IsNullOrEmpty(AppSettings.DbSettings.Password)) Virgo.DB.Config.Password = AppSettings.DbSettings.Password;
				Virgo.DB.Config.UseSsl = AppSettings.DbSettings.UseSsl;
				if (AppSettings.DbSettings.Servers.Length > 0)
				{
					List<MongoDB.Driver.MongoServerAddress> servers = new List<MongoDB.Driver.MongoServerAddress>();
					foreach (string s in AppSettings.DbSettings.Servers)
					{
						if (s.Contains(":") && int.TryParse(s.Split(":")[1], out int p))
						{
							string host = s.Split(":")[0];
							int port = p;
							servers.Add(new MongoDB.Driver.MongoServerAddress(host, port));
						}
					}
					Virgo.DB.Config.Servers = servers.ToArray();
				}
			}

			/* End Db Settings Related */



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

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
