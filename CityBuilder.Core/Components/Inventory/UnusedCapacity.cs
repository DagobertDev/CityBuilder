namespace CityBuilder.Core.Components.Inventory;

public readonly record struct UnusedCapacity(int Value)
{
	public static implicit operator int(UnusedCapacity unusedCapacity) => unusedCapacity.Value;
	public static implicit operator UnusedCapacity(int value) => new(value);
}
