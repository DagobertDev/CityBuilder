using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class CollectResourceStateSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, in CollectingResource collecting, [Changed] BehaviorState state)
	{
		if (!collecting.Resource.IsAlive)
		{
			return;
		}

		switch (state)
		{
			case Starting:
				entity.Set<Destination>(collecting.Resource.Get<Position>().Value);
				break;
			case Collecting:
				entity.Set<Waiting>(collecting.Resource.Get<Resource>().Difficulty);
				break;
			case Finished:
				collecting.Resource.Dispose();
				entity.Remove<CollectingResource>();
				entity.Set(BehaviorState.Deciding);
				break;
		}
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Collecting = Starting + 1;
	private const int Finished = Collecting + 1;
}
