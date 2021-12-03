using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(Sleeping))]
	[With(typeof(IsAtHome))]
	public sealed partial class SleepSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(float state, in Entity entity, ref Tiredness tiredness)
		{
			tiredness -= 10 * state;

			if (tiredness <= 0)
			{
				entity.Remove<Sleeping>();
				entity.Set<Idling>();
			}
		}
	}
}
