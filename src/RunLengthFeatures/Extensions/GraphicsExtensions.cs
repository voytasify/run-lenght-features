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
				var grayscaleMat = Mat.FromStream(ms, ImreadModes.GrayScale);
				var grayscaleImgStream = grayscaleMat.ToMemoryStream();
				var result = new BitmapImage();
				result.BeginInit();
				result.StreamSource = grayscaleImgStream;
				result.EndInit();
				return result;
			}
		}
	}
}
