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
		ref var job = ref workplace.Get<WorkProgress>();
		var difficulty = workplace.Get<Workplace>().Difficulty;
		job = job with { Value = job.Value + state / difficulty };
		workplace.NotifyChanged<WorkProgress>();
	}
}
