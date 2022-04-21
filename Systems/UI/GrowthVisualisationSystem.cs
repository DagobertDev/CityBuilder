using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI;

public sealed partial class GrowthVisualisationSystem : AEntitySetSystem<float>
{
	private const float MinimalScale = 0.3f;
	private const float MaximumScale = 0.7f;

	[Update]
	private static void Update(in Sprite sprite, [Added, Changed] in Growth growth)
	{
		var scale = growth == 1 ? 1 : MinimalScale + growth * (MaximumScale - MinimalScale);

		sprite.Scale = new Vector2(scale, scale);
	}
}
