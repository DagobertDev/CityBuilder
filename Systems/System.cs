using System;
using CityBuilder.Messages;
using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public class System
	{
		private readonly World _world;

		private System(World world)
		{
			_world = world;
		}

		public static ISystem<float> Create(World world)
		{
			world.Subscribe(new System(world));
			world.Subscribe(new ModLoader());

			return new SequentialSystem<float>(
				new SpriteSystem(world, CityBuilder.Instance.Map),
				new PositionSystem(world),
				new MovementSystem(world),
				new AISystem(world));
		}

		[Subscribe]
		private void On(in BlueprintPlacedMessage message)
		{
			message.Blueprint.Entity.CopyTo(_world).Set(message.Transform);

			/*var random = new Random();

			for (var i = 0; i < 1000; i++)
			{
				var transform = Transform2D.Identity;
				transform.origin = new Vector2(1000 * (float)random.NextDouble(), 1000 * (float)random.NextDouble());

				message.Blueprint.Entity.CopyTo(_world).Set(transform);
			}*/
		}
	}
}
