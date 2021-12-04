using System;
using System.Drawing;
using System.Numerics;
using DefaultEcs;

namespace CityBuilder.Components
{
	public class HitBox
	{
		public HitBox(Vector2 position, Vector2 size, Entity entity)
		{
			Value = new RectangleF((position - 0.5f * size).ToPoint(), new SizeF(size.X, size.Y));
			_entity = entity;
		}

		public RectangleF Value;
		private readonly Entity _entity;
		public Entity Entity => _entity.IsAlive ? _entity : throw new ApplicationException("Entity is not alive");
	}
}
