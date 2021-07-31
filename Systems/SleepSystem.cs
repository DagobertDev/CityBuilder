using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(IsAtHome))]
	public sealed partial class SleepSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(float state, ref Tiredness tiredness)
		{
			tiredness -= 10 * state;
		}
	}
}
