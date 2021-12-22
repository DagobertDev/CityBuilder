using System;
using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Core.Components;

public readonly struct Employee
{
	public Employee(Entity workplace)
	{
		_workplace = workplace;
		Location = workplace.Get<Position>().Value;
	}

	private readonly Entity _workplace;
	public Entity Workplace => _workplace.IsAlive ? _workplace : throw new ApplicationException("Entity is not alive");
	public Vector2 Location { get; }

	public override bool Equals(object other) => other is Employee employee && _workplace == employee._workplace;
	public override int GetHashCode() => _workplace.GetHashCode();
}