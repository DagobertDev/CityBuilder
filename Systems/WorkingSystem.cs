using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
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
}
