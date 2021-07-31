using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	[WhenRemoved(typeof(Destination))]
	public sealed partial class LocationSensorSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity, in Transform2D transform)
		{
			var position = transform.origin;

			if (entity.Has<Resident>() && entity.Get<Resident>().Location.DistanceSquaredTo(position) < 10)
			{
				entity.Set<IsAtHome>();
			}

			if (entity.Has<Employee>() && entity.Get<Employee>().Location.DistanceSquaredTo(position) < 10)
			{
				entity.Set<IsAtWorkplace>();
			}
		}
	}
}
