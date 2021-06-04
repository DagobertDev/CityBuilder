using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[WhenRemoved(typeof(Destination))]
	public class LocationSensorSystem : AEntitySetSystem<float>
	{
		public LocationSensorSystem(World world) : base(world, true) { }

		protected override void Update(float state, in Entity entity)
		{
			var position = entity.Get<Transform2D>().origin;

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
