using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems
{
	public sealed class HungerSystem : AComponentSystem<float, Hunger>
	{
		private readonly float _rate;

		public HungerSystem(World world, float rate) : base(world)
		{
			_rate = rate;
		}

		protected override void Update(float state, ref Hunger hunger)
		{
			hunger += state * _rate;
		}
	}
}
