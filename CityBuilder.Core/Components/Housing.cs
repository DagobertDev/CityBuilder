namespace CityBuilder.Core.Components
{
	public class Housing
	{
		public Housing(int maxBeds, int usedBeds = 0)
		{
			MaxBeds = maxBeds;
			UsedBeds = usedBeds;
		}

		public int MaxBeds { get; }
		public int UsedBeds { get; }
		public int EmptyBeds => MaxBeds - UsedBeds;
		public bool HasEmptyBeds => UsedBeds < MaxBeds;
	}
}
