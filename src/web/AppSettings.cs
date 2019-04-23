using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiriGuru.Web
{
	public static class AppSettings
	{
		public static Settings.AdminSettings AdminSettings { get; set; }
		public static Settings.DbSettings DbSettings { get; set; }
		public static Settings.FileSettings FileSettings { get; set; }
	}
}
