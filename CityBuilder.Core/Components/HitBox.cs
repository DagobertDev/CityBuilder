using System;
using System.Numerics;
using CityBuilder.Core.Types;
using DefaultEcs;

namespace CityBuilder.Core.Components;

public class HitBox
{
	public HitBox(Vector2 position, Vector2 size, Entity entity)
	{
		Value = new Rectangle(position - 0.5f * size, size);
		_entity = entity;
	}

	public Rectangle Value;
	private readonly Entity _entity;
	public Entity Entity => _entity.IsAlive ? _entity : throw new ApplicationException("Entity is not alive");
}
