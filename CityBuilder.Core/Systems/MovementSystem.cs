﻿using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public sealed partial class MovementSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(float state, in Entity entity, in Destination destinationComponent,
			ref Position transform, in Agent agent)
		{
			var destination = destinationComponent.Position;
			var speed = agent.Speed;

			transform = new Position(transform.Value.MoveToward(destination, state * speed));

			if (transform.Value == destination)
			{
				entity.Remove<Destination>();
				entity.Set<Idling>();
			}

			entity.Set(transform);
		}
	}
}