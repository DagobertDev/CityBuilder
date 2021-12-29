namespace CityBuilder.Core.Components.Behaviors;

public readonly record struct Waiting(float RemainingDuration)
{
	public static implicit operator float(Waiting waiting) => waiting.RemainingDuration;
	public static implicit operator Waiting(float remainingDuration) => new(remainingDuration);
}
