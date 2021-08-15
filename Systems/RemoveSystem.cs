using System.Linq;
using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs.System;
using UltimateQuadTree;

namespace CityBuilder.Systems
{
	[With(typeof(RemoveRequest))]
	public sealed partial class RemoveSystem : AEntitySetSystem<float>
	{
		[WorldComponent]
		private readonly QuadTree<HitBox> _quadTree;
		
		[Update] [UseBuffer]
		private void Update(in HitBox hitBox)
		{
			var box = hitBox;

			_quadTree.GetNearestObjects(hitBox)
				.Where(x => x.Value.Intersects(box.Value)).ToList()
				.ForEach(toBeRemoved => toBeRemoved.Entity.Dispose());
		}
	}
}
