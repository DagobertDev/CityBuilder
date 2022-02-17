namespace CityBuilder.Core.Components.Inventory;

public readonly record struct FutureUnusedCapacity(int Value)
{
	public static implicit operator int(FutureUnusedCapacity unusedCapacity) => unusedCapacity.Value;
	public static implicit operator FutureUnusedCapacity(int value) => new(value);
}
