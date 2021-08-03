using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public sealed partial class QuadTreeSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter]
		private readonly QuadTree<HitBox> _quadTree;

		public QuadTreeSystem(World world) : base(CreateEntityContainer(world, world), true)
		{
			_quadTree = World.Get<QuadTree<HitBox>>();
		}
		
		[Update] [UseBuffer]
		private void Update(in Entity entity, [Added] [Changed] in Transform2D transform, [Added] [Changed] in Sprite sprite)
		{
			var hitBox = new HitBox(transform.origin, sprite.Texture.GetSize(), entity);

			if (entity.Has<HitBox>())
			{
				var oldHitBox = entity.Get<HitBox>();
				_quadTree.Remove(oldHitBox);
			}

			_quadTree.Insert(hitBox);
			entity.Set(hitBox);
		}
	}
}
