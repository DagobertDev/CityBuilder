using System;
using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Agent))]
[Without(typeof(Employee))]
[With(typeof(Position))]
public class WorkSystem : AEntitySetSystem<float>
{
	private readonly EntitySet _emptyWorkplaces;
	private readonly EntityMultiMap<Employee> _employees;

	public WorkSystem(World world) : base(world, true)
	{
		World.SubscribeComponentAdded<Workplace>(Initialize);
		World.SubscribeComponentAdded<Employee>(AddEmployee);
		World.SubscribeComponentChanged<Employee>(ChangeEmployee);
		World.SubscribeComponentRemoved<Employee>(RemoveEmployee);

		World.SubscribeComponentRemoved((in Entity workplace, in Workplace _) =>
		{
			foreach (var employee in GetEmployees(workplace))
			{
				employee.Remove<Employee>();
			}
		});
			
		_emptyWorkplaces = world.GetEntities().With((in Workplace workplace) => workplace.HasEmptyWorkspace).With<Position>().AsSet();
		_employees = world.GetEntities().With<Position>().AsMultiMap<Employee>();
	}
		
	private static void Initialize(in Entity entity, in Workplace workplace)
	{
		if (!entity.Has<WorkProgress>())
		{
			entity.Set<WorkProgress>();
		}
	}


	protected override void Update(float state, in Entity worker)
	{
		var workplace = FindBestWorkplace(worker, _emptyWorkplaces.GetEntities());

		if (workplace.IsAlive)
		{
			worker.Set(new Employee(workplace));
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

	private static void AddEmployee(in Entity entity, in Employee employee)
	{
		var work = employee.Workplace;
		entity.SetSameAs<WorkProgress>(work);

		var workplace = work.Get<Workplace>();
		workplace = new Workplace(workplace.MaxEmployees, workplace.CurrentEmployees + 1);
		work.Set(workplace);
	}

	private static void RemoveEmployee(in Entity entity, in Employee employee)
	{
		entity.Remove<IsAtWorkplace>();
		entity.Remove<WorkProgress>();

		var work = employee.Workplace;
		var workplace = work.Get<Workplace>();
		workplace = new Workplace(workplace.MaxEmployees, workplace.CurrentEmployees - 1);
		work.Set(workplace);
	}

	private static void ChangeEmployee(in Entity entity, in Employee oldEmployee, in Employee newEmployee)
	{
		RemoveEmployee(entity, oldEmployee);
		AddEmployee(entity, newEmployee);
	}
		
	public ICollection<Entity> GetEmployees(Entity work)
	{
		if (_employees.TryGetEntities(new Employee(work), out var employees))
		{
			return employees.ToArray();
		}

		return Array.Empty<Entity>();
	}
}