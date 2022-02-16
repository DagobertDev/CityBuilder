namespace CityBuilder.Core.Components.Inventory;

public readonly record struct Amount(int Value)
{
	public static implicit operator int(Amount amount) => amount.Value;
	public static implicit operator Amount(int value) => new(value);
}
