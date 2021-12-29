using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Sleeping))]
[With(typeof(IsAtHome))]
public sealed partial class FinishedSleepSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Remove<Sleeping>();
		entity.Set<Idling>();
	}

	[WithPredicate]
	private bool Filter(in Tiredness tiredness) => tiredness <= 0;
}
