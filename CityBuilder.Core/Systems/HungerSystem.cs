using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Needs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class HungerSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update(float state, ref Hunger hunger, in HungerRate rate)
	{
		hunger += state * rate;
	}
}
