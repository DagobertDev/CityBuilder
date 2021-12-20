namespace CityBuilder.Core.Components.Inventory
{
	public readonly struct Amount
	{
		public Amount(int value)
		{
			Value = value;
		}
		
		public int Value { get; }
		
		public static implicit operator int(Amount tiredness) => tiredness.Value;
		public static implicit operator Amount(int value) => new(value);
	}
}
