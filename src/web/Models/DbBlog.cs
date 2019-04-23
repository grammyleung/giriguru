using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GiriGuru.Web.Models
{
	public class DbBlog
	{
		/// <summary>
		/// Global Unique Identifier
		/// </summary>
		public ObjectId _id { get; set; }
		/// <summary>
		/// Timestamp
		/// </summary>
		public DateTime _ts { get; set; }
		/// <summary>
		/// Global Unique Identifier
		/// </summary>
		public Guid Guid { get; set; }
		/// <summary>
		/// Blog Title
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// Blog Content Body (MicroBlog is plainText, Blog is MarkDown)
		/// </summary>
		public string Body { get; set; }
		/// <summary>
		/// If this blog is a microBlog
		/// </summary>
		public bool IsMicro { get; set; }
		/// <summary>
		/// If this blog is hidden from public
		/// </summary>
		public bool IsHidden { get; set; }
		/// <summary>
		/// If this blog has finished its editing.
		/// </summary>
		public bool IsEditing { get; set; }
		/// <summary>
		/// If this blog is deleted, even Admin can't see it in normal way.
		/// </summary>
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Key = Guid, Value = file name suffix (usually image type: jpg/png/gif/...)
		/// </summary>
		public KeyValuePair<Guid, string>[] Images { get; set; }
		/// <summary>
		/// Tag Guid List
		/// </summary>
		public Guid[] TagGuids { get; set; }
		/// <summary>
		/// Tag List
		/// </summary>
		public DbTag[] Tags { get; set; }
	}
}
