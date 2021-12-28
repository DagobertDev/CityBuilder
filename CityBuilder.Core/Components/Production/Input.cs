namespace CityBuilder.Core.Components.Production;

public readonly record struct Input(string Good, int Amount)
{
	public string Good { get; } = Good;
	public int Amount { get; } = Amount;
}
