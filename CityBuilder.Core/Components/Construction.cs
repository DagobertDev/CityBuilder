namespace CityBuilder.Core.Components
{
	public readonly struct Construction
	{
		public Construction(int workers, int duration)
		{
			Workers = workers;
			Duration = duration;
		}
		
		public int Workers { get; }
		public int Duration { get; }
	}
}
