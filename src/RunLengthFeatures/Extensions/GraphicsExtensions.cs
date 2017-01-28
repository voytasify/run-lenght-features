using System;
using System.IO;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace RunLengthFeatures.Extensions
{
	public static class GraphicsExtensions
	{
		public static Mat ToGrayscaleMat(this BitmapSource bmp)
		{
			var encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			using (var ms = new MemoryStream())
			{
				encoder.Save(ms);
				var newMat =  Mat.FromStream(ms, ImreadModes.GrayScale);
				PerformThresholding(newMat);
				return newMat;
			}
		}

		public static BitmapImage ToGrayscale(this BitmapImage bmp)
		{
			using (var grayscaleMat = bmp.ToGrayscaleMat())
			{
				var grayscaleImgStream = grayscaleMat.ToMemoryStream();
				var result = new BitmapImage();
				result.BeginInit();
				result.StreamSource = grayscaleImgStream;
				result.EndInit();
				return result;
			}
		}

		private static void PerformThresholding(Mat grayscaleMat)
		{
			for (var i = 0; i < grayscaleMat.Rows; i++)
			{
				for (var j = 0; j < grayscaleMat.Cols; j++)
				{
					var currentPixelValue = (int)grayscaleMat.At<char>(i, j);
					if (currentPixelValue >= 255d)
					{
						grayscaleMat.Set(i, j, (char)255);
						continue;
					}
					for (var multiplier = 1d; multiplier <= MainWindow.ShadesOfGray; multiplier ++)
					{
						var currentThreshold = (int) Math.Ceiling(multiplier/MainWindow.ShadesOfGray *255d);
						if (currentPixelValue < currentThreshold)
						{
							grayscaleMat.Set(i, j, (char)currentThreshold);
							break;
						}
					}
				}
			}
		}
	}
}
