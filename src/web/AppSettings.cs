using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Virgo.Web
{
	public static class AppSettings
	{
		public static Settings.UserSettings UserSettings { get; set; }
		public static Settings.JwtSettings JwtSettings { get; set; }
		public static Settings.DbSettings DbSettings { get; set; }
	}
}
