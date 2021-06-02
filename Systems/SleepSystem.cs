using CityBuilder.Models;
using CityBuilder.Models.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(Tiredness))]
	[With(typeof(IsAtHome))]
	public class SleepSystem : AEntitySetSystem<float>
	{
		public SleepSystem(World world) : base(world, true) { }

		protected override void Update(float state, in Entity entity)
		{
			entity.Set<Tiredness>(entity.Get<Tiredness>() - 10 * state);
		}
	}
}
