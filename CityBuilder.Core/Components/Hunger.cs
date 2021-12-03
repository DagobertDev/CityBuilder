namespace CityBuilder.Components
{
	public readonly struct Hunger
	{
		public Hunger(float value)
		{
			Value = value;
		}

		public float Value { get; }
		
		public static implicit operator float(Hunger tiredness) => tiredness.Value;
		public static implicit operator Hunger(float value) => new(value);
	}
}
