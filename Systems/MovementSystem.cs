using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	public sealed partial class MovementSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(float state, in Entity entity, in Destination destinationComponent,
			ref Transform2D transform, in Agent agent)
		{
			var destination = destinationComponent.Position;
			var speed = agent.Speed;

			transform.origin = transform.origin.MoveToward(destination, state * speed);

			if (transform.origin.DistanceSquaredTo(destination) < 100)
			{
				transform.origin = destination;

				entity.Remove<Destination>();
				entity.Set<Idling>();
			}

			entity.Set(transform);
		}
	}
}
