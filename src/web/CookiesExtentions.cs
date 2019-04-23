using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiriGuru.Web
{
	public static class CookiesExtensions
	{

		public static void SetString(HttpContext context, string key, string value, DateTime expires)
		{

			CookieOptions option = new CookieOptions();
			option.Expires = expires;

			context.Response.Cookies.Append(key, value, option);
		}
		public static string GetString(HttpContext context, string key)
		{
			return context.Request.Cookies[key];
		}

		public static void SetBoolean(HttpContext context, string key, bool value, DateTime expires)
		{
		
			CookieOptions option = new CookieOptions();
			option.Expires = expires;

			context.Response.Cookies.Append(key, value.ToString(), option);
		}
		public static bool? GetBoolean(HttpContext context, string key)
		{
			string value = context.Request.Cookies[key];

			if (value != null && bool.TryParse(value, out bool result))
			{
				return result;
			}
			else
				return null;
		}

	}
}
