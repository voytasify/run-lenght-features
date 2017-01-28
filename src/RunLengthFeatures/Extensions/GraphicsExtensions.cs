using System;
using System.IO;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace RunLengthFeatures.Extensions
{
	public static class GraphicsExtensions
	{
		public static BitmapImage ToGrayscale(this BitmapImage bmp)
		{
			var encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));
			using (var ms = new MemoryStream())
			{
				encoder.Save(ms);
				using (var grayscaleMat = Mat.FromStream(ms, ImreadModes.GrayScale))
				{
					PerformThresholding(grayscaleMat);
					var grayscaleImgStream = grayscaleMat.ToMemoryStream();
					var result = new BitmapImage();
					result.BeginInit();
					result.StreamSource = grayscaleImgStream;
					result.EndInit();
					return result;
				}
			}
		}

		private static void PerformThresholding(Mat grayscaleMat)
		{
			for (var i = 0; i < grayscaleMat.Rows; i++)
			{
				for (var j = 0; j < grayscaleMat.Cols; j++)
				{
					var currentPixelValue = (int)grayscaleMat.At<char>(i, j);
					for (var multiplier = 1d; multiplier <= MainWindow.ShadesOfGray; multiplier ++)
					{
						var currentThreshold = (int) Math.Round(multiplier/MainWindow.ShadesOfGray *255d);
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
