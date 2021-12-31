using CityBuilder.Core.Components;
using CityBuilder.Core.Messages;
using CityBuilder.Core.ModSupport;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Blueprint), typeof(Construction))]
public sealed partial class ConstructionSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private void Update(in Entity entity)
	{
		var blueprint = entity.Get<Blueprint>();
		var position = entity.Get<Position>();
		var rotation = entity.Get<Rotation>();
		entity.Dispose();

		World.Publish(new FinishedBuilding(blueprint, position, rotation));
	}

	[WithPredicate]
	private static bool Filter(in WorkProgress workProgress) => workProgress.Value >= 1;
}
