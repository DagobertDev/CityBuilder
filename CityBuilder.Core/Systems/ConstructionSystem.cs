using CityBuilder.Core.Components;
using CityBuilder.Core.Messages;
using CityBuilder.Core.ModSupport;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Blueprint))]
public sealed partial class ConstructionSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private void Update(in Entity entity, [Added] [Changed] in WorkProgress workProgress,
		in Construction construction)
	{
		if (workProgress >= construction.Duration)
		{
			var blueprint = entity.Get<Blueprint>();
			var position = entity.Get<Position>();
			entity.Dispose();

			World.Publish(new FinishedBuilding(blueprint, position));
		}
	}
}
