using CityBuilder.Core.Components;
using CityBuilder.ModSupport;

namespace CityBuilder.Messages
{
	public readonly struct BlueprintPlacedMessage
	{
		public BlueprintPlacedMessage(Blueprint blueprint, Position position, Rotation rotation)
		{
			Blueprint = blueprint;
			Position = position;
			Rotation = rotation;
		}

		public Blueprint Blueprint { get; }
		public Position Position { get; }
		public Rotation Rotation { get; }
	}
}
