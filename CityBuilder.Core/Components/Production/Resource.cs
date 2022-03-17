namespace CityBuilder.Core.Components.Production;

public readonly record struct Resource(string Type, float Difficulty,
	Resource.Status CollectionStatus = Resource.Status.None)
{
	public enum Status
	{
		None,
		CollectionRequested,
		Reserved,
	}
}
