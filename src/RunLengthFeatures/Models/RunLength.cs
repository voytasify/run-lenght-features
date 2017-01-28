namespace RunLengthFeatures.Models
{
	public class RunLength
	{
		public int Shade { get; set; }
		public int Length { get; set; }

		public override string ToString()
			=> $"{Shade};{Length}";
	}
}
