using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public sealed partial class WorkspaceInitSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity, [Added] [Changed] in Workplace workplace)
		{
			if (workplace.HasEmptyWorkspace)
			{
				entity.Set<EmptyWorkspace>();
			}

			else
			{
				entity.Remove<EmptyWorkspace>();
			}
		}
	}
}
