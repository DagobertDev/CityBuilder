using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public sealed partial class HousingInitSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity, [Added] [Changed] in Housing housing)
		{
			if (housing.HasEmptyBeds)
			{
				entity.Set<EmptyHousing>();
			}

			else
			{
				entity.Remove<EmptyHousing>();
			}
		}
	}
}
