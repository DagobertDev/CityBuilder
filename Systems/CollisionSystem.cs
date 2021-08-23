using System.Collections.Generic;
using System.Linq;
using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public sealed partial class CollisionSystem : AEntitySetSystem<float>
	{
		private readonly QuadTree<HitBox> _quadTree = new(-10000, -10000, 110000, 110000, new Bounds());

		public CollisionSystem(World world) : base(world, CreateEntityContainer, true)
		{
			World.SubscribeComponentRemoved((in Entity _, in HitBox hitBox) => _quadTree.Remove(hitBox));
		}

		[Update] [UseBuffer]
		private void Update(in Entity entity, [Added] [Changed] in Position transform,
			[Added] [Changed] in Sprite sprite)
		{
			var hitBox = new HitBox(transform.Value.ToGodotVector(), sprite.Texture.GetSize(), entity);

			if (entity.Has<HitBox>())
			{
				var oldHitBox = entity.Get<HitBox>();
				_quadTree.Remove(oldHitBox);
			}

			_quadTree.Insert(hitBox);
			entity.Set(hitBox);
		}

		public IEnumerable<Entity> GetEntities(HitBox hitBox) =>
			_quadTree.GetNearestObjects(hitBox).Where(box => box.Value.Intersects(hitBox.Value))
				.Select(box => box.Entity).ToList();

		public IEnumerable<Entity> GetEntities(Vector2 position) =>
			_quadTree.GetNearestObjects(new HitBox(position, Vector2.One, default))
				.Where(box => box.Value.HasPoint(position)).Select(box => box.Entity).ToList();

		private class Bounds : IQuadTreeObjectBounds<HitBox>
		{
			public double GetLeft(HitBox hitBox) => hitBox.Value.Position.x;
			public double GetRight(HitBox hitBox) => hitBox.Value.Position.x + hitBox.Value.Size.x;
			public double GetTop(HitBox hitBox) => hitBox.Value.Position.y;
			public double GetBottom(HitBox hitBox) => hitBox.Value.Position.y + hitBox.Value.Size.y;
		}
	}
}
