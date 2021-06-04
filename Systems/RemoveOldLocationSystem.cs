using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[WhenAdded(typeof(Destination))]
	public class RemoveOldLocationSystem : AEntitySetSystem<float>
	{
		public RemoveOldLocationSystem(World world) : base(world, true) { }

		protected override void Update(float state, in Entity entity)
		{
			entity.Remove<IsAtHome>();
			entity.Remove<IsAtWorkplace>();
		}
	}
}
