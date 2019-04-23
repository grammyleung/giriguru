using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiriGuru.Web.Utilities
{
	public static class DateAndTime
	{

		#region UnixTimeStamp
		public static DateTime UnixTimeStampToDateTimeSeconds(this double unixTimeStampSeconds)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStampSeconds);
			return dtDateTime;
		}

		public static DateTime UnixTimeStampToDateTimeMilliseconds(this double unixTimeStampMilliseconds)
		{
			// Unix timestamp is seconds past epoch
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddMilliseconds(unixTimeStampMilliseconds);
			return dtDateTime;
		}

		public static double UnixTimestampFromDateTimeSeconds(this DateTime dt)
		{
			return (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}

		public static double UnixTimestampFromDateTimeMilliseconds(this DateTime dt)
		{
			return (dt - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
		}
		#endregion

		#region BsonTimestamp

		public static DateTime BsonTimestampToDateTime(BsonTimestamp ts)
		{
			return UnixTimeStampToDateTimeSeconds(ts.Timestamp);
		}

		public static BsonTimestamp DateTimeToBsonTimestamp(DateTime dt)
		{
			return new BsonTimestamp((int)UnixTimestampFromDateTimeSeconds(dt), 1);
		}

		#endregion

		#region Start Of Period

		public static DateTime StartOfWeek(DateTime dt)
		{
			return dt.Date.AddDays(-(int)dt.Date.DayOfWeek);
		}

		public static double StartOfWeekUnixTimeStamp(DateTime dt)
		{
			return UnixTimestampFromDateTimeMilliseconds(StartOfWeek(dt));
		}

		public static DateTime StartOfMonth(DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, 1);
		}

		public static double StartOfMonthUnixTimeStamp(DateTime dt)
		{
			return UnixTimestampFromDateTimeMilliseconds(StartOfMonth(dt));
		}

		#endregion

		#region TimeZone

		public static List<string> TimeZoneIdList()
		{
			List<string> returnValue = new List<string>();

			var provider = NodaTime.DateTimeZoneProviders.Tzdb;
			foreach (var id in provider.Ids)
			{
				var zone = provider[id];
				returnValue.Add(zone.Id);
			}

			return returnValue;
		}

		public static Dictionary<string, double> TimeZoneIdOffsetList()
		{
			Dictionary<string, double> returnValue = new Dictionary<string, double>();

			var provider = NodaTime.DateTimeZoneProviders.Tzdb;
			foreach (var id in provider.Ids)
			{
				var zone = provider[id];
				returnValue.Add(zone.Id, zone.GetUtcOffset(NodaTime.SystemClock.Instance.GetCurrentInstant()).ToTimeSpan().TotalHours);
			}

			return returnValue;
		}

		#endregion

	}
}
