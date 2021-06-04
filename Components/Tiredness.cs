namespace CityBuilder.Components
{
	public readonly struct Tiredness
	{
		public Tiredness(float value)
		{
			Value = value;
		}

		public float Value { get; }

		public static implicit operator float(Tiredness tiredness) => tiredness.Value;
		public static implicit operator Tiredness(float value) => new(value);
	}
}
