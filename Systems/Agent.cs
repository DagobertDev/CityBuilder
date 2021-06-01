namespace CityBuilder.Systems
{
	public readonly struct Agent
	{
		public Agent(float speed)
		{
			Speed = speed;
		}

		public float Speed { get; }
	}
}
