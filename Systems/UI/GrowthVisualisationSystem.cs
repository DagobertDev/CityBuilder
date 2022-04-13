using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI;

public sealed partial class GrowthVisualisationSystem : AEntitySetSystem<float>
{
	private const float MinimalAlpha = 0.3f;
	private const float MaximumAlpha = 0.7f;

	[Update]
	private static void Update(in Sprite sprite, [Added, Changed] in Growth growth)
	{
		var modulate = sprite.SelfModulate;

		modulate.a = growth == 1 ? 1 : MinimalAlpha + growth * (MaximumAlpha - MinimalAlpha);

		sprite.SelfModulate = modulate;
	}
}
