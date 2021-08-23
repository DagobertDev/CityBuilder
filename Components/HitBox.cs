using System;
using DefaultEcs;
using Godot;

namespace CityBuilder.Components
{
	public class HitBox
	{
		public HitBox(Vector2 position, Vector2 size, Entity entity)
		{
			Value = new Rect2(position - 0.5f * size, size);
			_entity = entity;
		}

		public Rect2 Value;
		private readonly Entity _entity;
		public Entity Entity => _entity.IsAlive ? _entity : throw new ApplicationException("Entity is not alive");
	}
}
