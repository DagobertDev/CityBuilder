namespace CityBuilder.Core.Components.Behaviors;

public readonly struct Waiting
{
	public Waiting(float duration)
	{
		RemainingDuration = duration;
	}
		
	public float RemainingDuration { get; }
}