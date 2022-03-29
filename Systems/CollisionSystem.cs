using System.Collections.Generic;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Core.Components;
using CityBuilder.Core.Systems;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;
using Vector2 = System.Numerics.Vector2;
using World = DefaultEcs.World;

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
			var oldBoundingBox = entity.Get<HitBox>();
			_quadTree.Remove(oldBoundingBox);
		}

		var boundingBox = rotation.Value switch
		{
			0 or 180 => new HitBox(position.Value, size, entity),
			90 or 270 => new HitBox(position.Value, new Vector2(size.Value.Y, size.Value.X), entity),
			_ => CalculateBoundingBox(position, size, entity),
		};

		_quadTree.Insert(boundingBox);
		entity.Set(boundingBox);

		// This is not the minimal bounding box, just a bounding box
		static HitBox CalculateBoundingBox(Position position, Size size, Entity entity)
		{
			var diagonalLength = size.Value.Length();
			return new HitBox(position.Value, Vector2.One * diagonalLength, entity);
		}
	}

	public IEnumerable<Entity> GetEntities(HitBox boundingBox)
	{
		var possibleCollisions = _quadTree.GetNearestObjects(boundingBox)
			.Where(box => box.Value.Intersects(boundingBox.Value));

		Shape2D shape = new RectangleShape2D
		{
			Extents = boundingBox.Value.Size.ToGodotVector() / 2,
		};

		var transform =
			new Transform2D(0, (boundingBox.Value.Position + 0.5f * boundingBox.Value.Size).ToGodotVector());

		var result = new List<Entity>();

		foreach (var otherHitBox in possibleCollisions)
		{
			var otherShape = new RectangleShape2D
				{ Extents = otherHitBox.Entity.Get<Size>().Value.ToGodotVector() / 2 };
			var otherTransform = new Transform2D(otherHitBox.Entity.Get<Rotation>().Radians,
				otherHitBox.Entity.Get<Position>().Value.ToGodotVector());

			if (shape.Collide(transform, otherShape, otherTransform))
			{
				result.Add(otherHitBox.Entity);
			}
		}

		return result;
	}

	public IEnumerable<Entity> GetEntities(Vector2 position) =>
		GetEntities(new HitBox(position, Vector2.One * 2, default));

	private class Bounds : IQuadTreeObjectBounds<HitBox>
	{
		public double GetLeft(HitBox hitBox) => hitBox.Value.Position.X;
		public double GetRight(HitBox hitBox) => hitBox.Value.Position.X + hitBox.Value.Size.X;
		public double GetTop(HitBox hitBox) => hitBox.Value.Position.Y;
		public double GetBottom(HitBox hitBox) => hitBox.Value.Position.Y + hitBox.Value.Size.Y;
	}
}
