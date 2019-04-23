using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiriGuru.Web
{
	public static class SessionExtensions
	{
		public static void SetObject(this ISession session, string key, object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}

		public static T GetObject<T>(this ISession session, string key)
		{
			T returnValue = default(T);
			string value = session.GetString(key);

			string count = session.Keys.Where(i => i == "Authen").Count().ToString();

			Console.WriteLine("TryGetObject GetObject<T>. SessionID: " + session.Id + "| value is null: " + (value == null) + "| key: " + key + "| count: " + count);
			if (value != null)
			{
				Console.WriteLine("TryGetObject GetObject<T>. value = " + value.ToString());
			}
			else
			{
				foreach (var k in session.Keys)
				{
					Console.WriteLine("session(" + k + ")=");
					try
					{
						Console.WriteLine(session.GetString(k));
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
			}

			returnValue = (value == null || value == string.Empty) ? default(T) : JsonConvert.DeserializeObject<T>(value);
			return returnValue;
		}

		public static bool TryGetObject<T>(this ISession session, string key, out T value)
		{
			bool returnValue = false;

			Console.WriteLine("TryGetObject Begin: session is null? " + (session == null).ToString() + " | key = " + key);
			value = default(T);
			try
			{
				value = session.GetObject<T>(key);

				Console.WriteLine("TryGetObject Procession. value is null: " + (value == null));

				returnValue = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("TryGetObject: \n" + ex.ToString());
			}

			Console.WriteLine("TryGetObject End: returnValue=" + returnValue.ToString());
			return returnValue;
		}

		//public static void SetString(this ISession session, string key, string value)
		//{
		//	session.SetString(key, value);
		//}

		//public static string GetString(this ISession session, string key)
		//{
		//	var value = session.GetString(key);
		//	return value == null ? string.Empty : value;
		//}

		//public static bool TryGetString(this ISession session, string key, out string value)
		//{
		//	bool returnValue = false;

		//	value = string.Empty;
		//	try
		//	{
		//		value = session.GetString(key);
		//		returnValue = true;
		//	}
		//	catch (Exception) { }

		//	return returnValue;
		//}



		public static void SetBoolean(this ISession session, string key, bool value)
		{
			session.Set(key, BitConverter.GetBytes(value));
		}

		public static bool? GetBoolean(this ISession session, string key)
		{
			var data = session.Get(key);
			if (data == null)
			{
				return null;
			}
			return BitConverter.ToBoolean(data, 0);
		}

		public static void SetDouble(this ISession session, string key, double value)
		{
			session.Set(key, BitConverter.GetBytes(value));
		}

		public static double? GetDouble(this ISession session, string key)
		{
			var data = session.Get(key);
			if (data == null)
			{
				return null;
			}
			return BitConverter.ToDouble(data, 0);
		}
	}
}
