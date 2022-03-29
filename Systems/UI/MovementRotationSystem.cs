using CityBuilder.Core;
using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems.UI;

public sealed partial class MovementRotationSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, [Added, Changed] Waypoint waypoint, in Position position)
	{
		var angle = position.Value.AngleTo(waypoint.Position);
		entity.Set<Rotation>((int)angle);
	}
}
