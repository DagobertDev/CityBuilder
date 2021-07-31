using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.GodotInterface
{
	[With(typeof(Sprite))]
	public sealed partial class SpritePositionSystem : AEntitySetSystem<float>
	{
		[Update]
		private static void Update([Added] [Changed] in Sprite sprite, [Added] [Changed] in Transform2D transform)
		{
			sprite.Transform = transform;
		}
	}
}
