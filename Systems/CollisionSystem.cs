using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Systems;
using DefaultEcs;
using DefaultEcs.System;
using UltimateQuadTree;

namespace CityBuilder.Systems;

public sealed partial class CollisionSystem : AEntitySetSystem<float>, ICollisionSystem
{
	private readonly QuadTree<HitBox> _quadTree;

	public CollisionSystem(World world, int x, int y, int width, int height) : base(
		world, CreateEntityContainer, true)
	{
		_quadTree = new QuadTree<HitBox>(x, y, width, height, new Bounds());

		World.SubscribeComponentRemoved((in Entity _, in HitBox hitBox) => _quadTree.Remove(hitBox));
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, [Added, Changed] in Position position,
		[Added, Changed] in Size size, [Added, Changed] Rotation rotation)
	{
		if (entity.Has<HitBox>())
		{
			var oldHitBox = entity.Get<HitBox>();
			_quadTree.Remove(oldHitBox);
		}

		var hitBox = rotation.Value is 90 or 270
			? new HitBox(position.Value, new Vector2(size.Value.Y, size.Value.X), entity)
			: new HitBox(position.Value, size, entity);

		_quadTree.Insert(hitBox);
		entity.Set(hitBox);
	}

	public IEnumerable<Entity> GetEntities(HitBox hitBox) =>
		_quadTree.GetNearestObjects(hitBox).Where(box => box.Value.Intersects(hitBox.Value))
			.Select(box => box.Entity).ToList();

	public IEnumerable<Entity> GetEntities(Vector2 position) =>
		_quadTree.GetNearestObjects(new HitBox(position, Vector2.One, default))
			.Where(box => box.Value.Contains(position)).Select(box => box.Entity).ToList();

	private class Bounds : IQuadTreeObjectBounds<HitBox>
	{
		public double GetLeft(HitBox hitBox) => hitBox.Value.Position.X;
		public double GetRight(HitBox hitBox) => hitBox.Value.Position.X + hitBox.Value.Size.X;
		public double GetTop(HitBox hitBox) => hitBox.Value.Position.Y;
		public double GetBottom(HitBox hitBox) => hitBox.Value.Position.Y + hitBox.Value.Size.Y;
	}
}
