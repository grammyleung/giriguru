using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GiriGuru.Web.Models
{
	public class UploadImageResult
	{
		/*
		{
			success : 0 | 1,           // 0 表示上传失败，1 表示上传成功
			message : "提示的信息，上传成功或上传失败及错误信息等。",
			url     : "图片地址"        // 上传成功时才返回
		} 
		 */

		public Result Success { get; set; }
		public string Message { get; set; }
		public string Url { get; set; }


		public enum Result
		{
			Fail = 0,
			Success = 1
		}
	}

	public enum UploadImageType
	{
		BlogCover = 1,
		BlogImage = 2
	}
}
