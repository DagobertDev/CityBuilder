using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class DoNothingSystem : AEntitySetSystem<float>
{
	private const int WaitDuration = 5;

	[Update, UseBuffer]
	private static void Update(in Entity entity)
	{
		entity.Set<Waiting>(WaitDuration);
	}
}
