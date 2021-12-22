using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Waiting))] [With(typeof(IsAtWorkplace))]
public sealed partial class WorkingSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(float state, in Entity entity, in Employee employee, ref WorkProgress workProgress)
	{
		workProgress += state;
		employee.Workplace.NotifyChanged<WorkProgress>();
	}
}