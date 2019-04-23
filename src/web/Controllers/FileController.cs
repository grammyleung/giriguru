using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GiriGuru.Web.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using System.IO;
using GiriGuru.Web.Utilities;
using Microsoft.Extensions.Localization;

namespace GiriGuru.Web.Controllers
{
	[Route("[controller]")]

	public class FileController : Controller
	{
		private readonly IStringLocalizer<FileController> _localizer;

		public FileController(IStringLocalizer<FileController> localizer)
		{
			_localizer = localizer;
		}

		#region BlogCover

		[Route("Blog/{guidBlog}/cover.jpg")]
		[HttpGet]
		public ActionResult<object> GetBlogCover(Guid guidBlog)
		{
			try
			{
				string imagePath = Path.Combine(AppSettings.FileSettings.FileBlogPath(guidBlog), "cover.jpg");
				if (System.IO.File.Exists(imagePath))
				{
					return PhysicalFile(imagePath, "image/jpg");
				}
				else
					return StatusCode(404);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[Route("Blog/{guidBlog}/cover_p.jpg")]
		[HttpGet]
		public ActionResult<object> GetBlogCoverPreview(Guid guidBlog)
		{
			try
			{
				string imagePath = Path.Combine(AppSettings.FileSettings.FileBlogPath(guidBlog), "cover_p.jpg");
				if (System.IO.File.Exists(imagePath))
				{
					return PhysicalFile(imagePath, "image/jpg");
				}
				else
					return StatusCode(404);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[Route("Blog/{guidBlog}/cover_t.jpg")]
		[HttpGet]
		public ActionResult<object> GetBlogCoverThumbnail(Guid guidBlog)
		{
			try
			{
				string imagePath = Path.Combine(AppSettings.FileSettings.FileBlogPath(guidBlog), "cover_t.jpg");
				if (System.IO.File.Exists(imagePath))
				{
					return PhysicalFile(imagePath, "image/jpg");
				}
				else
					return StatusCode(404);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		#endregion


		#region BlogImage

		[Route("Blog/{guidBlog}/{guidImage}.{extention}")]
		[HttpGet]
		public ActionResult<object> GetBlogImage(Guid guidBlog, Guid guidImage, string extention)
		{
			try
			{
				string imagePath = Path.Combine(AppSettings.FileSettings.FileBlogPath(guidBlog), guidImage.ToString() + "." + extention);
				if (System.IO.File.Exists(imagePath))
				{
					return PhysicalFile(imagePath, "image/" + extention);
				}
				else
					return StatusCode(404);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		[Route("Blog/{guidBlog}/{guidImage}_t.{extention}")]
		[HttpGet]
		public ActionResult<object> GetBlogImageThumbnail(Guid guidBlog, Guid guidImage, string extention)
		{
			try
			{
				string imagePath = Path.Combine(AppSettings.FileSettings.FileBlogPath(guidBlog), guidImage.ToString() + "_t." + extention);
				if (System.IO.File.Exists(imagePath))
				{
					return PhysicalFile(imagePath, "image/" + extention);
				}
				else
					return StatusCode(404);
			}
			catch (Exception ex)
			{
				return StatusCode(500, ex.Message);
			}
		}

		#endregion

	}
}
