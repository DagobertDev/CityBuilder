using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class WaitingSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(float state, in Entity entity, ref Waiting waiting)
	{
		waiting -= state;
		entity.NotifyChanged<Waiting>();
	}
}
