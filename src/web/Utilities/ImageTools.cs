using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

namespace GiriGuru.Web.Utilities
{
	public class ImageTools
	{
		public enum ImageFormat
		{
			bmp,
			jpeg,
			gif,
			tiff,
			png,
			unknown
		}

		public static ImageFormat GetImageFormat(byte[] bytes)
		{
			var bmp = Encoding.ASCII.GetBytes("BM");     // BMP
			var gif = Encoding.ASCII.GetBytes("GIF");    // GIF
			var png = new byte[] { 137, 80, 78, 71 };    // PNG
			var tiff = new byte[] { 73, 73, 42 };         // TIFF
			var tiff2 = new byte[] { 77, 77, 42 };         // TIFF
			var jpeg = new byte[] { 255, 216, 255, 224 }; // jpeg
			var jpeg2 = new byte[] { 255, 216, 255, 225 }; // jpeg canon

			if (bmp.SequenceEqual(bytes.Take(bmp.Length)))
				return ImageFormat.bmp;

			if (gif.SequenceEqual(bytes.Take(gif.Length)))
				return ImageFormat.gif;

			if (png.SequenceEqual(bytes.Take(png.Length)))
				return ImageFormat.png;

			if (tiff.SequenceEqual(bytes.Take(tiff.Length)))
				return ImageFormat.tiff;

			if (tiff2.SequenceEqual(bytes.Take(tiff2.Length)))
				return ImageFormat.tiff;

			if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
				return ImageFormat.jpeg;

			if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
				return ImageFormat.jpeg;

			return ImageFormat.unknown;
		}

		public void SaveThumnail(string ReadFileName, string saveFileName, int size)
		{
			using (Image<Rgba32> image = Image.Load(ReadFileName))
			{
				int cropSize = Math.Min(image.Width, image.Height);
				int offsetX, offsetY, widthToCrop = Math.Min(image.Width, image.Height), heightToCrop = widthToCrop;
				if (image.Width > image.Height)
				{
					offsetX = (int)((image.Width - image.Height) / 2);
					offsetY = 0;
				}
				else
				{
					offsetX = 0;
					offsetY = (int)((image.Height - image.Width) / 2);
				}
				SixLabors.Primitives.Rectangle rec = new SixLabors.Primitives.Rectangle(offsetX, offsetY, widthToCrop, heightToCrop);
				image.Mutate(x => x
					.Crop(rec)
					.Resize(size, size)
					);
				image.Save(saveFileName); // Automatic encoder selected based on extension.
			}
			GC.Collect();
		}

		public void SaveThumnail(string ReadFileName, string saveFileName, int width, int height)
		{
			using (Image<Rgba32> image = Image.Load(ReadFileName))
			{
				//int cropSize = Math.Min(image.Width, image.Height);
				int offsetX, offsetY;
				int widthToCrop, heightToCrop;

				//Math.Min(image.Width, image.Height)

				if ((double)image.Width / (double)image.Height > (double)width / (double)height)
				{
					widthToCrop = (int)((double)image.Height * ((double)width / (double)height));
					heightToCrop = image.Height;
				}
				else
				{
					widthToCrop = image.Width;
					heightToCrop = (int)((double)image.Width * ((double)height / (double)width));
				}

				offsetX = (int)((image.Width - widthToCrop) / 2);
				offsetY = (int)((image.Height - heightToCrop) / 2);

				SixLabors.Primitives.Rectangle rec = new SixLabors.Primitives.Rectangle(offsetX, offsetY, widthToCrop, heightToCrop);
				image.Mutate(x => x
					.Crop(rec)
					.Resize(width, height)
					);
				image.Save(saveFileName); // Automatic encoder selected based on extension.
			}
			GC.Collect();
		}


	}
}
