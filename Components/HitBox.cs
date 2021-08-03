using DefaultEcs;
using Godot;
using UltimateQuadTree;

namespace CityBuilder.Components
{
	public class HitBox
	{
		public HitBox(Vector2 position, Vector2 size, Entity entity)
		{
			X = position.x;
			Y = position.y;
			Width = size.x;
			Height = size.y;
			Entity = entity;
		}
			
		public float X { get; }
		public float Y {get;}
		public float Height { get; }
		public float Width { get; }
		public Entity Entity { get; }
	}
	
	public class HitBoxBounds : IQuadTreeObjectBounds<HitBox>
	{
		public double GetLeft(HitBox hitBox) => hitBox.X;

		public double GetRight(HitBox hitBox) => hitBox.X + hitBox.Width;

		public double GetTop(HitBox hitBox) => hitBox.Y;

		public double GetBottom(HitBox hitBox) => hitBox.Y + hitBox.Height;
	}
}
