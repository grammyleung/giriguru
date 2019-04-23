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
	public class BlogController : Controller
	{
		private readonly IStringLocalizer<BlogController> _localizer;

		public BlogController(IStringLocalizer<BlogController> localizer)
		{
			_localizer = localizer;
		}

		#region Edit

		public IActionResult Edit()
		{
			return View();
		}

		public IActionResult Edit_SimpleMDE()
		{
			return View();
		}

		#endregion

		#region Upload

		/// <summary>
		/// Uploads an image to the server.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> UploadImage([FromQuery]string guid)
		{
			if (HttpContext.Request.Form.Files.Count > 0)
			{
				if (Guid.TryParse(guid, out Guid guidBlog))
				{
					IFormFile file = HttpContext.Request.Form.Files[0];
					return new ObjectResult(await UploadImage(UploadImageType.BlogImage, guidBlog, file));
				}
				else
				{
					return BadRequest();
				}
			}
			else
				return BadRequest();
		}

		public async Task<UploadImageResult> UploadImage(UploadImageType type, Guid guid, IFormFile file)
		{
			UploadImageResult returnResult = new UploadImageResult();
			if (CheckIfImageFile(file))
			{
				return await WriteFile(type, guid, file);
			}
			else
			{
				returnResult.Success = UploadImageResult.Result.Fail;
				returnResult.Message = _localizer["uploaded_file_is_not_image"];
				return returnResult;
			}
		}

		/// <summary>
		/// Method to check if file is image file
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private bool CheckIfImageFile(IFormFile file)
		{
			byte[] fileBytes;
			using (var ms = new MemoryStream())
			{
				file.CopyTo(ms);
				fileBytes = ms.ToArray();
			}

			return ImageTools.GetImageFormat(fileBytes) != ImageTools.ImageFormat.unknown;
		}

		/// <summary>
		/// Method to write file onto the disk
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public async Task<UploadImageResult> WriteFile(UploadImageType type, Guid guid, IFormFile file)
		{
			UploadImageResult returnResult;
			string fileName;
			try
			{
				var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
				//extension = ".jpg";
				//Create a new Name for the file due to security reasons.
				string fileGuid = Guid.NewGuid().ToString();
				/*
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);
				*/

				if (!Directory.Exists(AppSettings.FileSettings.FileBlogRootFull))
				{
					Directory.CreateDirectory(AppSettings.FileSettings.FileBlogRootFull);
				}
				var pathBlog = AppSettings.FileSettings.FileBlogPath(guid);
				if (!Directory.Exists(pathBlog))
				{
					Directory.CreateDirectory(pathBlog);
				}

				switch (type)
				{
					case UploadImageType.BlogCover:
						{
							fileName = "cover.jpg";
							string fileName_Preview = "cover_p.jpg";
							string fileName_Thumnail = "cover_t.jpg";
							var path = Path.Combine(pathBlog, fileName);
							var path_preview = Path.Combine(pathBlog, fileName_Preview);
							var path_thumnail = Path.Combine(pathBlog, fileName_Thumnail);
							using (var bits = new FileStream(path, FileMode.Create))
							{
								await file.CopyToAsync(bits);
							}
							Utilities.ImageTools imageTools = new ImageTools();
							imageTools.SaveThumnail(path, path_preview, 628, 355);
							imageTools.SaveThumnail(path, path_thumnail, 80);

							returnResult = new UploadImageResult()
							{
								Success = UploadImageResult.Result.Success,
								Message = _localizer["upload_image_success_true"],
								Url = AppSettings.FileSettings.FileBlogUrlRoot(guid) + fileName
							};

						}
						break;
					case UploadImageType.BlogImage:
						{
							fileName = fileGuid + extension;
							//string fileName_Preview = fileGuid + "_p" + extension;
							string fileName_Thumnail = fileGuid + "_t" + extension;

							var path = Path.Combine(pathBlog, fileName);
							//var path_preview = Path.Combine(pathBlog, fileName_Preview);
							var path_thumnail = Path.Combine(pathBlog, fileName_Thumnail);
							using (var bits = new FileStream(path, FileMode.Create))
							{
								await file.CopyToAsync(bits);
							}
							Utilities.ImageTools imageTools = new ImageTools();
							//imageTools.SaveThumnail(path, path_preview, 628, 355);
							imageTools.SaveThumnail(path, path_thumnail, 80);

							returnResult = new UploadImageResult()
							{
								Success = UploadImageResult.Result.Success,
								Message = _localizer["upload_image_success_true"],
								Url = AppSettings.FileSettings.FileBlogUrlRoot(guid) + fileName
							};
						}
						break;
					default:
						{
							returnResult = new UploadImageResult()
							{
								Success = UploadImageResult.Result.Fail
							};
						};
						break;
				}



				/*
				var pathGroup = Path.Combine(pathStarting, guid.ToString().Substring(0, 2) + "/");
				if (!Directory.Exists(pathGroup))
				{
					Directory.CreateDirectory(pathGroup);
				}

				var pathPlace = Path.Combine(pathGroup, guid.ToString() + "/");
				if (!Directory.Exists(pathPlace))
				{
					Directory.CreateDirectory(pathPlace);
				}
				*/
			}
			catch (Exception e)
			{
				returnResult = new UploadImageResult()
				{
					Success = UploadImageResult.Result.Fail,
					Message = e.Message
				};
			}

			if (returnResult == null)
			{
				returnResult = new UploadImageResult()
				{
					Success = UploadImageResult.Result.Fail
				};
			}
			return returnResult;
		}

		#endregion

		#region Tag

		public IActionResult Tag()
		{
			ViewBlogTag model = new ViewBlogTag();

			model = LoadViewBlogTag(GetQueriedGuid());

			return View(model);
		}

		#region Group

		[HttpPost]
		public IActionResult TagGroup(string formGuid, string formNameEn, string formNameZh)
		{
			ViewBlogTag model = new ViewBlogTag();

			if (Guid.TryParse(formGuid, out Guid guid))
			{
				model = LoadViewBlogTag(guid);

				if (model != null)
				{
					bool isAddingNew = (model.SelectedTagGroup.Guid == Guid.Empty);
					if (formNameEn.Trim() == string.Empty || formNameZh.Trim() == string.Empty)
					{
						model.ResultMessage = _localizer["result-group-name-empty"];
					}
					else
					{
						Data.Db.Tag tag = new Data.Db.Tag();
						if (tag.CheckGroupDuplication(guid, formNameEn.Trim()) || tag.CheckGroupDuplication(guid, formNameZh.Trim()))
							model.ResultMessage = _localizer["result-group-name-duplicated"];
						else
						{
							model.SelectedTagGroup.Name_en = formNameEn.Trim();
							model.SelectedTagGroup.Name_zh = formNameZh.Trim();
							tag.Update(model.SelectedTagGroup);
						}
					}

					if (isAddingNew)
						return LocalRedirect("/Blog/Tag");
					else
						return LocalRedirect("/Blog/Tag?" + model.SelectedTagGroup.Guid.ToString());
				}
				else
					return LocalRedirect("/Blog/Tag");
			}
			else
				return LocalRedirect("/Blog/Tag");
		}

		private Guid? GetQueriedGuid()
		{
			Guid? guid = null;
			string queriedGuid = HttpContext.Request.QueryString.ToString().Replace("?", "");
			if (Guid.TryParse(queriedGuid, out Guid _guid))
			{
				guid = _guid;
			}
			return guid;
		}

		private ViewBlogTag LoadViewBlogTag(Guid? groupGuid)
		{
			ViewBlogTag model = new ViewBlogTag();

			Data.Db.Tag tag = new Data.Db.Tag();
			model.TagGroups = tag.ReadAll();

			if (groupGuid == Guid.Empty)
			{
				model.SelectedTagGroup = new DbTagGroup();
			}
			else
				model.SelectedTagGroup = model.TagGroups.Where(i => i.Guid == groupGuid).SingleOrDefault();

			if (model.SelectedTagGroup != null)
			{
				if (model.SelectedTagGroup.Guid == Guid.Empty)
				{
					model.FormTitle = _localizer["form-title-new"];
					model.FormSubmit = _localizer["form-submit-new"];
				}
				else
				{
					model.FormTitle = _localizer["form-title-edit"];
					model.FormSubmit = _localizer["form-submit-edit"];
				}
			}

			return model;
		}

		[HttpPost]
		public IActionResult TagGroupDelete(string guid)
		{
			if (Guid.TryParse(guid, out Guid groupGuid))
			{
				Data.Db.Tag tag = new Data.Db.Tag();
				DbTagGroup dbTagGroup = tag.ReadAsGroup(groupGuid);

				if (dbTagGroup != null)
				{
					if (dbTagGroup.Tags == null || dbTagGroup.Tags.Count == 0)
					{
						tag.Delete(groupGuid);

						return Ok();
					}
					else
						return BadRequest("NotEmpty");
				}
				else
					return NotFound();
			}
			else
				return BadRequest("InvalidGuid");
		}

		#endregion

		#region Tag


		[HttpPost]
		public IActionResult TagInsert(string formGuid, string formNameEn, string formNameZh)
		{
			Data.Db.Tag tag = new Data.Db.Tag();
			DbTagGroup group = null;

			if (Guid.TryParse(formGuid, out Guid guid))
			{
				group = tag.ReadAsGroup(guid);

				if (group != null)
				{
					if (formNameEn.Trim() != string.Empty && formNameZh.Trim() != string.Empty)
					{
						if (!tag.CheckTagDuplication(guid, formNameEn.Trim()) || tag.CheckTagDuplication(guid, formNameZh.Trim()))
						{
							if (group.Tags == null)
							{
								group.Tags = new List<DbTag>();
							}
							group.Tags.Add(new DbTag()
							{
								Guid = Guid.NewGuid(),
								Name_en = formNameEn.Trim(),
								Name_zh = formNameZh.Trim(),
								_tsInsert = DateTime.UtcNow,
								_tsUpdate = DateTime.UtcNow
							});

							tag.Update(group);
						}
					}

					return LocalRedirect("/Blog/Tag?" + guid.ToString());
				}
				else
					return LocalRedirect("/Blog/Tag");
			}
			else
				return LocalRedirect("/Blog/Tag");
		}

		[HttpPost]
		public IActionResult TagUpdate(string formGroupGuid, string formTagGuid, string formNameEn, string formNameZh)
		{
			Data.Db.Tag tag = new Data.Db.Tag();
			DbTagGroup group = null;

			if (Guid.TryParse(formGroupGuid, out Guid groupGuid))
			{
				group = tag.ReadAsGroup(groupGuid);

				if (group != null)
				{
					if (formNameEn.Trim() != string.Empty && formNameZh.Trim() != string.Empty)
					{
						if (Guid.TryParse(formTagGuid, out Guid tagGuid))
						{
							if (!tag.CheckTagDuplication(tagGuid, formNameEn.Trim()) || tag.CheckTagDuplication(tagGuid, formNameZh.Trim()))
							{
								if (group.Tags != null)
								{
									foreach (DbTag t in group.Tags)
									{
										if (t.Guid == tagGuid)
										{
											t._tsUpdate = DateTime.UtcNow;
											t.Name_en = formNameEn.Trim();
											t.Name_zh = formNameZh.Trim();
										}
									}

									tag.Update(group);
								}
							}
						}
					}

					return LocalRedirect("/Blog/Tag?" + groupGuid.ToString());
				}
				else
					return LocalRedirect("/Blog/Tag");
			}
			else
				return LocalRedirect("/Blog/Tag");
		}

		[HttpPost]
		public IActionResult TagDelete(string group_guid, string tag_guid)
		{
			if (Guid.TryParse(group_guid, out Guid groupGuid) && Guid.TryParse(tag_guid, out Guid tagGuid))
			{
				Data.Db.Tag tag = new Data.Db.Tag();
				DbTagGroup dbTagGroup = tag.ReadAsGroup(groupGuid);

				if (dbTagGroup != null && dbTagGroup.Tags != null)
				{
					DbTag dbTag = dbTagGroup.Tags.Where(i => i.Guid == tagGuid).FirstOrDefault();
					if (dbTag != null)
					{
						dbTagGroup.Tags.Remove(dbTag);
						tag.Update(dbTagGroup);
						return Ok();
					}
					else
						return NotFound();
				}
				else
					return NotFound();
			}
			else
				return BadRequest("InvalidGuid");
		}

		[HttpGet]
		public void TagMove(string tagGuid, string groupGuid)
		{
			string redirectToGroupGuid = string.Empty;

			if (Guid.TryParse(tagGuid, out Guid _tagGuid) && Guid.TryParse(groupGuid, out Guid _groupGuid))
			{
				Data.Db.Tag collectionTag = new Data.Db.Tag();
				DbTagGroup newGroup = collectionTag.ReadAsGroup(_groupGuid);

				if (newGroup != null)
				{
					if (newGroup.Tags == null)
					{
						newGroup.Tags = new List<DbTag>();
					}
					DbTagGroup oldGroup = collectionTag.ReadAsGroupWithTag(_tagGuid);
					if (oldGroup != null)
					{
						DbTag tag = oldGroup.Tags.Where(i => i.Guid == _tagGuid).FirstOrDefault();
						if (tag != null)
						{
							oldGroup.Tags.Remove(tag);
							newGroup.Tags.Add(tag);
							collectionTag.Update(oldGroup);
							collectionTag.Update(newGroup);

							redirectToGroupGuid = oldGroup.Guid.ToString();
						}
					}
				}
			}

			Response.Redirect("/Blog/Tag?" + redirectToGroupGuid);
		}


		#endregion


		#endregion



	}
}
