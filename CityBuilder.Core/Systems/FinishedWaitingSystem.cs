using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class FinishedWaitingSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Remove<Waiting>();
		entity.Set<Idling>();
	}

	[WithPredicate]
	private bool Filter(in Waiting waiting) => waiting.RemainingDuration <= 0;
}
