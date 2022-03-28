using CityBuilder.Components;
using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI;

public sealed partial class SpritePositionSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update(in Sprite sprite, [Changed] in Position position)
	{
		sprite.Position = position.Value.ToGodotVector();
	}
}
