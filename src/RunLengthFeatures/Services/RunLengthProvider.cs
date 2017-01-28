using System.Collections.Generic;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using RunLengthFeatures.Extensions;
using RunLengthFeatures.Models;

namespace RunLengthFeatures.Services
{
	public class RunLengthProvider
	{
		public IEnumerable<RunLength> ComputeRunLengths(BitmapSource bitmap, Rect subarea)
		{
			var result = new List<RunLength>();
			using (var originalMat = bitmap.ToGrayscaleMat())
			using (var submatrix = originalMat.SubMat(subarea))
			{
				for (var i = 0; i < submatrix.Rows; i++)
				{
					var currentRun = new RunLength();
					for (var j = 0; j < submatrix.Cols; j++)
					{
						var pixelColor = (int)submatrix.At<char>(i, j);
						if (currentRun.Length == 0)
						{
							currentRun.Length++;
							currentRun.Shade = pixelColor;
						}
						else if (currentRun.Shade == pixelColor)
							currentRun.Length++;
						else
						{
							result.Add(currentRun);
							currentRun = new RunLength { Length = 1, Shade = pixelColor };
						}
					}
					result.Add(currentRun);
				}
			}
			return result;
		}
	}
}
