using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GiriGuru.Web.Models
{
	public class DbTagGroup
	{
		/// <summary>
		/// Global Unique Identifier
		/// </summary>
		public ObjectId _id { get; set; }
		/// <summary>
		/// Timestamp
		/// </summary>
		public DateTime _tsInsert { get; set; }
		/// <summary>
		/// Timestamp
		/// </summary>
		public DateTime _tsUpdate { get; set; }
		/// <summary>
		/// Global Unique Identifier
		/// </summary>
		public Guid Guid { get; set; }
		/// <summary>
		/// Tag Group Name in English
		/// </summary>
		public String Name_en { get; set; }
		/// <summary>
		/// Tag Group Name in Chinese
		/// </summary>
		public String Name_zh { get; set; }
		/// <summary>
		/// Tags in Group
		/// </summary>
		public List<DbTag> Tags { get; set; }
	}
}
