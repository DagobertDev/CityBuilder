using System;
using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(Agent))]
	[Without(typeof(Employee))]
	[With(typeof(Position))]
	public class WorkSystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _emptyWorkplaces;

		public WorkSystem(World world) : base(world, true)
		{
			_emptyWorkplaces = world.GetEntities().With<Workplace>().With<EmptyWorkspace>().With<Position>().AsSet();
		}

		protected override void Update(float state, in Entity worker)
		{
			var workplace = FindBestWorkplace(worker, _emptyWorkplaces.GetEntities());

			if (workplace.IsAlive)
			{
				AddWorker(worker, workplace);
			}
		}

		private static Entity FindBestWorkplace(Entity worker, ReadOnlySpan<Entity> workplaces)
		{
			var position = worker.Get<Position>().Value;
			Entity bestMatch = default;
			var currentDistance = float.MaxValue;

			foreach (var workplace in workplaces)
			{
				var workplacePosition = workplace.Get<Position>().Value;
				var distance = position.DistanceSquaredTo(workplacePosition);

				if (distance < currentDistance)
				{
					bestMatch = workplace;
					currentDistance = distance;
				}
			}

			return bestMatch;
		}

		private static void AddWorker(Entity employee, Entity workplace)
		{
			if (employee.Has<Employee>())
			{
				throw new NotImplementedException("Employee already has workplace");
			}

			employee.Set(new Employee(workplace));

			var workplaceComponent = workplace.Get<Workplace>();

			workplaceComponent.Employees.Add(employee);

			if (!workplaceComponent.HasEmptyWorkspace)
			{
				workplace.Remove<EmptyWorkspace>();
			}
		}
	}
}
