using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[WhenAdded(typeof(Destination))]
	public sealed partial class RemoveOldLocationSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity)
		{
			entity.Remove<IsAtHome>();
			entity.Remove<IsAtWorkplace>();
		}
	}
}
