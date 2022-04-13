using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(GrowthRate))]
[Without(typeof(Growth))]
public sealed partial class GrowthInitSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set<Growth>(0);
	}
}
