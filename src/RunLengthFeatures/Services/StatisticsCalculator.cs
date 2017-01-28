using System;
using System.Collections.Generic;
using System.Linq;
using RunLengthFeatures.Models;

namespace RunLengthFeatures.Services
{
	public class StatisticsCalculator
	{
		public double ShortPrimitiveEmphasis(IReadOnlyList<RunLength> runs)
		{
			var shades = runs.Select(run => run.Shade).OrderBy(shade => shade).Distinct().ToList();
			var maxLength = runs.Max(run => run.Length);
			var res = 0d;
			foreach (var shade in shades)
			{
				for (var i = 1; i <= maxLength; i++)
				{
					res += runs.Count(run => run.Length == i && run.Shade == shade) / Math.Pow(i, 2);
				}
			}
			return res/runs.Count;
		}

		public double LongPrimitiveEmphasis(IReadOnlyList<RunLength> runs)
		{
			var shades = runs.Select(run => run.Shade).OrderBy(shade => shade).Distinct().ToList();
			var maxLength = runs.Max(run => run.Length);
			var res = 0d;
			foreach (var shade in shades)
			{
				for (var i = 1; i <= maxLength; i++)
				{
					res += runs.Count(run => run.Length == i && run.Shade == shade) * Math.Pow(i, 2);
				}
			}
			return res / runs.Count;
		}

		public double GrayLevelUniformity(IReadOnlyList<RunLength> runs)
		{
			var shades = runs.Select(run => run.Shade).OrderBy(shade => shade).Distinct().ToList();
			var maxLength = runs.Max(run => run.Length);
			var res = 0d;
			foreach (var shade in shades)
			{
				var subsum = 0d;
				for (var i = 1; i <= maxLength; i++)
				{
					subsum += runs.Count(run => run.Length == i && run.Shade == shade) * Math.Pow(i, 2);
				}
				res += Math.Pow(subsum, 2);
			}
			return res / runs.Count;
		}

		public double PrimitiveLengthUniformity(IReadOnlyList<RunLength> runs)
		{
			var shades = runs.Select(run => run.Shade).OrderBy(shade => shade).Distinct().ToList();
			var maxLength = runs.Max(run => run.Length);
			var res = 0d;
			for (var i = 1; i <= maxLength; i++)
			{
				var subsum = shades.Sum(shade => runs.Count(run => run.Length == i && run.Shade == shade)*Math.Pow(i, 2));
				res += Math.Pow(subsum, 2);
			}
			return res / runs.Count;
		}
	}
}
