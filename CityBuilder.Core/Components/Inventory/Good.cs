namespace CityBuilder.Core.Components.Inventory
{
	public readonly struct Good
	{
		public Good(string name)
		{
			Name = name;
		}
		
		public string Name { get; }
	}
}
