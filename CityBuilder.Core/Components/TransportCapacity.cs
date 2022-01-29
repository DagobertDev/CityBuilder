namespace CityBuilder.Core.Components;

public readonly record struct TransportCapacity(int Value)
{
	public static implicit operator int(TransportCapacity capacity) => capacity.Value;
	public static implicit operator TransportCapacity(int value) => new(value);
}
