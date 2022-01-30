using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Needs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class TirednessSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update(float state, ref Tiredness tiredness, in TirednessRate rate)
	{
		tiredness += state * rate;
	}
}
