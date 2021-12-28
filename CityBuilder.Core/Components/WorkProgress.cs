namespace CityBuilder.Core.Components;

public readonly record struct WorkProgress(float Value)
{
	public static implicit operator float(WorkProgress tiredness) => tiredness.Value;
	public static implicit operator WorkProgress(float value) => new(value);
}
