using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;

namespace CityBuilder.Systems
{
	public sealed partial class QuadTreeSystem : AEntitySetSystem<float>
	{
		[WorldComponent]
		private readonly QuadTree<HitBox> _quadTree;

		[Update] [UseBuffer]
		private void Update(in Entity entity, [Added] [Changed] in Position transform, [Added] [Changed] in Sprite sprite)
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
	}
}
