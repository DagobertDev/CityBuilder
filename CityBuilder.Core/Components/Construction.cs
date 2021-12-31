namespace CityBuilder.Core.Components;

public readonly record struct Construction(int Workers, int Duration)
{
	public Workplace ToWorkplace() => new(Workers, Duration);
}
