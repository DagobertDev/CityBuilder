using CityBuilder.Core.ModSupport;

namespace CityBuilder.Core.Messages;

public readonly struct BlueprintSelectedMessage
{
	public BlueprintSelectedMessage(Blueprint blueprint)
	{
		Blueprint = blueprint;
	}

	public Blueprint Blueprint { get; }
}
