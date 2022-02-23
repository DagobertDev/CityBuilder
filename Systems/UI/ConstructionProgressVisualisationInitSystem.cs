using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI
{
	[Without(typeof(ProgressBar))]
	[With(typeof(Construction))]
	public sealed partial class ConstructionProgressVisualisationInitSystem : AEntitySetSystem<float>
	{
		[Update]
		[UseBuffer]
		private static void Update(in Entity entity, in Sprite sprite)
		{
			var size = new Vector2(100, 30);

			var zIndex = new Node2D
			{
				ZIndex = 2,
			};

			var progressBar = new ProgressBar
			{
				MaxValue = 1,
				RectMinSize = size,
				RectPosition = -0.5f * size,
			};

			sprite.AddChild(zIndex);
			zIndex.AddChild(progressBar);
			entity.Set(progressBar);

			zIndex.GlobalRotation = 0;
		}
	}
}
