using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public sealed class HungerSystem : AComponentSystem<float, Hunger>
	{
		public HungerSystem(World world) : base(world) { }

		protected override void Update(float state, ref Hunger hunger)
		{
			hunger += state;
		}
	}
}
