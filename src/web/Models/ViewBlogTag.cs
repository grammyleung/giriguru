using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace GiriGuru.Web.Models
{
	public class ViewBlogTag
	{
		public DbTagGroup[] TagGroups { get; set; }
		public DbTagGroup SelectedTagGroup { get; set; }
		public string ResultMessage { get; set; }
		public string FormTitle { get; set; }
		public string FormSubmit { get; set; }
	}
}
