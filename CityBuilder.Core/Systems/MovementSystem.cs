using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class MovementSystem : AEntitySetSystem<float>
{
	[Update]
	[UseBuffer]
	private static void Update(float state, in Entity entity, in Destination destinationComponent,
		ref Position transform, in Speed speed)
	{
		var destination = destinationComponent.Position;

		transform = new Position(transform.Value.MoveToward(destination, state * speed));

		if (transform.Value == destination)
		{
			entity.Remove<Destination>();
			entity.Get<BehaviorState>().Next(out var next);
			entity.Set(next);
		}

		entity.Set(transform);
	}
}
