using CityBuilder.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.GodotInterface
{
	public sealed partial class SpritePositionSystem : AEntitySetSystem<float>
	{
		[Update]
		private static void Update(in Sprite sprite, [Changed] in Position position)
		{
			sprite.Position = position.Value.ToGodotVector();
		}
	}
}
