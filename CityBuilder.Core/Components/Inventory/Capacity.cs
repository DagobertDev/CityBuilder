namespace CityBuilder.Core.Components.Inventory;

public readonly record struct Capacity(int Value)
{
	public static implicit operator int(Capacity capacity) => capacity.Value;
	public static implicit operator Capacity(int value) => new(value);
}
