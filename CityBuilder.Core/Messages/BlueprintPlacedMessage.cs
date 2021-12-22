using CityBuilder.Core.Components;
using CityBuilder.Core.ModSupport;

namespace CityBuilder.Core.Messages;

public readonly struct BlueprintPlacedMessage
{
	public BlueprintPlacedMessage(Blueprint blueprint, Position position)
	{
		Blueprint = blueprint;
		Position = position;
	}

	public Blueprint Blueprint { get; }
	public Position Position { get; }
}