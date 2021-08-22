using CityBuilder.Components;
using CityBuilder.ModSupport;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	[With(typeof(Blueprint))]
	public sealed partial class ConstructionSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private void Update(in Entity entity, [Added] [Changed] in WorkProgress workProgress,
			in Construction construction, in Sprite sprite)
		{
			var modulate = sprite.Modulate;
			modulate.a = 0.2f + 0.6f * workProgress / construction.Duration;
			sprite.Modulate = modulate;

			if (workProgress >= construction.Duration)
			{
				var blueprint = entity.Get<Blueprint>();
				var position = entity.Get<Position>();
				entity.Dispose();

				var newEntity = World.CreateEntity();
				newEntity.Set(position);
				blueprint.Populate(entity);
			}
		}
	}
}
