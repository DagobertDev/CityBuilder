using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI
{ 
	[Without(typeof(ProgressBar))]
	public sealed partial class ConstructionProgressVisualisationInitSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity, in Construction construction, in Sprite sprite)
		{
			var size = new Vector2(100, 30);

			var zIndex = new Node2D();
			zIndex.ZIndex = 2;
			
			var progressBar = new ProgressBar
			{
				MaxValue = construction.Duration,
				RectMinSize = size,
				RectPosition = -0.5f * size
			};
			
			sprite.AddChild(zIndex);
			zIndex.AddChild(progressBar);
			entity.Set(progressBar);
		}
	}
}
