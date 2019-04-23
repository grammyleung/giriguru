using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GiriGuru.Web.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace GiriGuru.Web.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public IActionResult SetLanguage(string culture, string returnUrl)
		{
			string cookieName = CookieRequestCultureProvider.DefaultCookieName;
			string cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
			Response.Cookies.Append(cookieName, cookieValue,
				new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			);

			/*
			var a = Request.Cookies[cookieName];

			CookieOptions option = new CookieOptions();
			option.Expires = DateTime.MaxValue;
			Response.Cookies.Append("testCookie", DateTime.Now.ToString(), option);

			var b = Request.Cookies["testCookie"];
			*/

			return LocalRedirect(returnUrl);
		}


		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(string formEmail, string formPassword, bool formRemember, string returnUrl)
		{
			if (formEmail.Trim().ToLower() == AppSettings.AdminSettings.AdminEmail.ToLower() && formPassword == AppSettings.AdminSettings.AdminPassword)
			{
				DateTime expiry = DateTime.Now.AddHours(1);
				if (formRemember)
				{
					expiry = DateTime.Now.AddDays(7);
				}
				CookiesExtensions.SetBoolean(HttpContext, "IsAdmin", true, expiry);
				return LocalRedirect(returnUrl);
			}
			else
				return LocalRedirect("/Home/Login");
		}

		public IActionResult Logout(string returnUrl)
		{
			CookiesExtensions.SetBoolean(HttpContext, "IsAdmin", false, DateTime.Now.AddHours(1));
			if (string.IsNullOrEmpty(returnUrl))
			{
				returnUrl = "~/";
			}
			return LocalRedirect(returnUrl);
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
