using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(Destination))]
	[With(typeof(Transform2D))]
	[With(typeof(Agent))]
	public class MovementSystem : AEntitySetSystem<float>
	{
		public MovementSystem(World world) : base(world, true) { }

		protected override void Update(float state, in Entity entity)
		{
			var destination = entity.Get<Destination>().Position;
			var transform = entity.Get<Transform2D>();

			transform.origin = transform.origin.MoveToward(destination, state * entity.Get<Agent>().Speed);

			if (transform.origin.DistanceSquaredTo(destination) < 100)
			{
				transform.origin = destination;

				entity.Remove<Destination>();
			}

			entity.Set(transform);
		}
	}
}
