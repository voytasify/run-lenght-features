using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RunLengthFeatures.Extensions
{
	public static class GraphicsExtensions
	{
		public static FormatConvertedBitmap ToGrayscale(this BitmapImage bmp)
		{
			var grayscaleBmp = new BitmapImage();

			grayscaleBmp.BeginInit();
			grayscaleBmp.UriSource = bmp.UriSource;
			grayscaleBmp.EndInit();

			var grayscaleFormatBmp = new FormatConvertedBitmap();

			grayscaleFormatBmp.BeginInit();
			grayscaleFormatBmp.Source = grayscaleBmp;
			grayscaleFormatBmp.DestinationFormat = PixelFormats.Gray2;
			grayscaleFormatBmp.EndInit();

			return grayscaleFormatBmp;
		}
	}
}
