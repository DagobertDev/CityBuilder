using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[WhenRemoved(typeof(Destination))]
public sealed partial class LocationSensorSystem : AEntitySetSystem<float>
{
	public LocationSensorSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		world.SubscribeComponentRemoved<Destination>(RemoveOldLocations);
	}

	private static void RemoveOldLocations(in Entity entity, in Destination destination)
	{
		entity.Remove<IsAtHome>();
		entity.Remove<IsAtWorkplace>();
	}
		
	[Update] [UseBuffer]
	private static void Update(in Entity entity, in Position positionComponent)
	{
		var position = positionComponent.Value;

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