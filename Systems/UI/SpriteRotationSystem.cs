using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI;

public sealed partial class SpriteRotationSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update([Added, Changed] in Sprite sprite, [Added, Changed] in Rotation rotation)
	{
		sprite.RotationDegrees = rotation;
	}
}
