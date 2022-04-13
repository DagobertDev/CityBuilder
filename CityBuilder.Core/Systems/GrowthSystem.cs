using System;
using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class GrowthSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, float delta, in Growth growth, in GrowthRate growthRate)
	{
		entity.Set<Growth>(Math.Min(growth + growthRate * delta, 1));
	}

	[WithPredicate]
	private static bool Filter(in Growth growth) => growth < 1;
}
