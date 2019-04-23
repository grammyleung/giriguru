using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace GiriGuru.Web.Settings
{
	public class DbSettings
	{
		public string DatabaseName { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string[] Servers { get; set; }
		public bool UseSsl { get; set; }

		public SecureString PasswordSecureString => new NetworkCredential("", Password).SecurePassword;
		public MongoServerAddress[] ServerAddresses => ServerStringsToAddresses(Servers);

		private MongoServerAddress[] ServerStringsToAddresses(string[] servers)
		{
			MongoServerAddress[] returnValue = { };
			if (servers.Length > 0)
			{
				List<MongoDB.Driver.MongoServerAddress> addresses = new List<MongoDB.Driver.MongoServerAddress>();
				foreach (string s in servers)
				{
					if (s.Contains(":") && int.TryParse(s.Split(":")[1], out int p))
					{
						string host = s.Split(":")[0];
						int port = p;
						addresses.Add(new MongoDB.Driver.MongoServerAddress(host, port));
					}
				}
				returnValue = addresses.ToArray();
			}
			return returnValue;
		}

		public MongoClientSettings GetClientSettings()
		{
			MongoClientSettings returnValue = new MongoClientSettings()
			{
				Servers = this.ServerAddresses,
				UseSsl = this.UseSsl
			};

			if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
				returnValue.Credential = MongoCredential.CreateCredential("admin", this.Username, this.PasswordSecureString);

			return returnValue;
		}
	}
}
