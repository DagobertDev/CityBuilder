using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems
{
	[With(typeof(RemoveRequest))]
	public sealed partial class RemoveSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter]
		private readonly ICollisionSystem _quadTree;
		
		[Update] [UseBuffer]
		private void Update(in HitBox hitBox)
		{
			foreach (var entity in _quadTree.GetEntities(hitBox))
			{
				entity.Dispose();
			}
		}
	}
}
