using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;

namespace GiriGuru.Web.Settings
{
	public class FileSettings
	{
		public string FileRoot { get; set; }
		public string FileBlogRoot { get; set; }
		public string FileBlogRootFull => FileRoot + FileBlogRoot;
		public string FileBlogPath(Guid guidBlog) => Path.Combine(FileBlogRootFull, guidBlog.ToString() + "/");
		public string FileBlogUrlRoot(Guid guidBlog) => "/File/Blog/" + guidBlog.ToString() + "/";
	}
}
