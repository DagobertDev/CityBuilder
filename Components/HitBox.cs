using System;
using DefaultEcs;
using Godot;
using UltimateQuadTree;

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
	
	public class HitBoxBounds : IQuadTreeObjectBounds<HitBox>
	{
		public double GetLeft(HitBox hitBox) => hitBox.Value.Position.x;

		public double GetRight(HitBox hitBox) => hitBox.Value.Position.x + hitBox.Value.Size.x;

		public double GetTop(HitBox hitBox) => hitBox.Value.Position.y;

		public double GetBottom(HitBox hitBox) => hitBox.Value.Position.y + hitBox.Value.Size.y;
	}
}
