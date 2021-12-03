using CityBuilder.Components;
using CityBuilder.ModSupport;

namespace CityBuilder.Messages
{
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
}
