using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class MovementSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(float state, in Entity entity, in Waypoint destinationComponent,
		ref Position position, in Speed speed)
	{
		var waypoint = destinationComponent.Position;

		position = new Position(position.Value.MoveToward(waypoint, state * speed));

		if (position.Value == waypoint)
		{
			position = waypoint;
			entity.Remove<Waypoint>();
		}

		entity.Set(position);
	}
}
