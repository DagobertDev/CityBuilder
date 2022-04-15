namespace CityBuilder.Components;

public readonly record struct NavigationOutlineId(int Value)
{
	public static implicit operator int(NavigationOutlineId id) => id.Value;
	public static implicit operator NavigationOutlineId(int id) => new(id);
}
