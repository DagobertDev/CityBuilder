using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using UltimateQuadTree;

namespace CityBuilder.Systems
{
	public sealed partial class CollisionSystem<T> : AEntitySetSystem<float>, ICollisionSystem
	{
		private readonly QuadTree<HitBox> _quadTree = new(-10000, -10000, 110000, 110000, new Bounds());
		private readonly Func<T, Vector2> _hitBoxFactory;

		public CollisionSystem(World world, Func<T, Vector2> hitBoxFactory) : base(world, CreateEntityContainer, true)
		{
			_hitBoxFactory = hitBoxFactory;
			
			World.SubscribeComponentRemoved((in Entity _, in HitBox hitBox) => _quadTree.Remove(hitBox));
		}

		[Update] [UseBuffer]
		private void Update(in Entity entity, [Added] [Changed] in Position transform,
			[Added] [Changed] in T sprite)
		{
			var hitBox = new HitBox(transform.Value, _hitBoxFactory(sprite), entity);

			if (entity.Has<HitBox>())
			{
				var oldHitBox = entity.Get<HitBox>();
				_quadTree.Remove(oldHitBox);
			}

			_quadTree.Insert(hitBox);
			entity.Set(hitBox);
		}

		public IEnumerable<Entity> GetEntities(HitBox hitBox) =>
			_quadTree.GetNearestObjects(hitBox).Where(box => box.Value.IntersectsWith(hitBox.Value))
				.Select(box => box.Entity).ToList();

		public IEnumerable<Entity> GetEntities(Vector2 position) =>
			_quadTree.GetNearestObjects(new HitBox(position, Vector2.One, default))
				.Where(box => box.Value.Contains(position.ToPoint())).Select(box => box.Entity).ToList();

		private class Bounds : IQuadTreeObjectBounds<HitBox>
		{
			public double GetLeft(HitBox hitBox) => hitBox.Value.Left;
			public double GetRight(HitBox hitBox) => hitBox.Value.Right;
			public double GetTop(HitBox hitBox) => hitBox.Value.Top;
			public double GetBottom(HitBox hitBox) => hitBox.Value.Bottom;
		}
	}
}
