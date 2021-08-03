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
			Entity = entity;
		}

		public Rect2 Value;
		public Entity Entity { get; }
	}
	
	public class HitBoxBounds : IQuadTreeObjectBounds<HitBox>
	{
		public double GetLeft(HitBox hitBox) => hitBox.Value.Position.x;

		public double GetRight(HitBox hitBox) => hitBox.Value.Position.x + hitBox.Value.Size.x;

		public double GetTop(HitBox hitBox) => hitBox.Value.Position.y;

		public double GetBottom(HitBox hitBox) => hitBox.Value.Position.y + hitBox.Value.Size.y;
	}
}
