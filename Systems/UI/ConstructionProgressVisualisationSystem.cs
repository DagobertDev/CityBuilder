using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI
{
	public sealed partial class ConstructionProgressVisualisationSystem : AEntitySetSystem<float>
	{
		[Update]
		private static void Update([Added] [Changed] in WorkProgress workProgress,
			in Construction construction, in Sprite sprite, [Added] [Changed] in ProgressBar progressBar)
		{
			var modulate = sprite.SelfModulate;
			modulate.a = 0.2f + 0.6f * workProgress / construction.Duration;
			sprite.SelfModulate = modulate;

			progressBar.Value = workProgress;
		}
	}
}
