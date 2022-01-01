namespace CityBuilder.Core.Components;

public readonly record struct InventoryPriority(Priority Value)
{
	public static implicit operator Priority(InventoryPriority tiredness) => tiredness.Value;
	public static implicit operator InventoryPriority(Priority value) => new(value);
}

public enum Priority
{
	Low, Medium, High,
}
