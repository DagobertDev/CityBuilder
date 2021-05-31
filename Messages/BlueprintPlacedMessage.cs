using CityBuilder.Models;
using Godot;

namespace CityBuilder.Messages
{
	public readonly struct BlueprintPlacedMessage
	{
		public BlueprintPlacedMessage(Blueprint blueprint, Transform2D transform)
		{
			Blueprint = blueprint;
			Transform = transform;
		}

		public Blueprint Blueprint { get; }
		public Transform2D Transform { get; }
	}
}
