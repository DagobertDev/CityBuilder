using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Flags;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Waiting))] [With(typeof(IsAtWorkplace))]
public sealed partial class WorkingSystem : AEntitySetSystem<float>
{
	[Update] [UseBuffer]
	private static void Update(float state, in Employee employee)
	{
		var workplace = employee.Workplace;
		workplace.Get<WorkProgress>() += state;
		workplace.NotifyChanged<WorkProgress>();
	}
}
