namespace CityBuilder.Core.Components;

public readonly record struct WorkProgress(float Value)
{
	public static implicit operator float(WorkProgress workProgress) => workProgress.Value;
	public static implicit operator WorkProgress(float value) => new(value);
}
