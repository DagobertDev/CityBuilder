using System.Linq;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[WhenAdded(typeof(CanNotWorkReason))]
public sealed partial class ValidateWorkingDecisionSystem : AEntitySetSystem<float>
{
	private WorkSystem? _workSystem;
	private WorkSystem WorkSystem => _workSystem ??= World.Get<WorkSystem>();

	public ValidateWorkingDecisionSystem(World world) : base(world, CreateEntityContainer, null, 0)
	{
		world.SubscribeComponentRemoved((in Entity employee, in Employee _) =>
		{
			if (employee.Has<Behavior>() && employee.Get<Behavior>() == Behavior.Working)
			{
				WorkingStateSystem.Cancel(employee);
			}
		});
	}

	[Update]
	private void Update(in Entity workplace)
	{
		foreach (var employee in WorkSystem.GetEmployees(workplace)
					 .Where(e => e.Has<Behavior>() && e.Get<Behavior>() == Behavior.Working))
		{
			WorkingStateSystem.Cancel(employee);
		}
	}
}
