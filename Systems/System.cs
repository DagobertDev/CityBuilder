using DefaultEcs;
using DefaultEcs.System;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public static class System
	{
		public static ISystem<float> Create(World world)
		{
			world.Subscribe(new ModLoader());

			return new SequentialSystem<float>();
		}
	}
}
