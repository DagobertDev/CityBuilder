namespace CityBuilder.Core.Components.Inventory;

public readonly record struct FutureAmount(int Value)
{
	public static implicit operator int(FutureAmount amount) => amount.Value;
	public static implicit operator FutureAmount(int value) => new(value);
}
