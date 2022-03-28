using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI;

[With(typeof(Construction))]
public sealed partial class ConstructionProgressVisualisationSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update([Added, Changed] in WorkProgress workProgress, in Sprite sprite,
		[Added, Changed] in ProgressBar progressBar)
	{
		var modulate = sprite.SelfModulate;
		modulate.a = 0.2f + 0.6f * workProgress.Value;
		sprite.SelfModulate = modulate;

		progressBar.Value = workProgress.Value;
	}
}
