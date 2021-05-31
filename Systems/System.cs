using CityBuilder.Messages;
using DefaultEcs;
using DefaultEcs.System;
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
				new SpriteSystem(world, CityBuilder.Root),
				new PositionSystem(world));
		}

		[Subscribe]
		private void On(in BlueprintPlacedMessage message)
		{
			message.Blueprint.Entity.CopyTo(_world).Set(message.Transform);
		}
	}
}
