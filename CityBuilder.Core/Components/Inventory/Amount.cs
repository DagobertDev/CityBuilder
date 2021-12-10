namespace CityBuilder.Components.Inventory
{
	public readonly struct Amount
	{
		public Amount(int value)
		{
			Value = value;
		}
		
		public int Value { get; }
	}
}
