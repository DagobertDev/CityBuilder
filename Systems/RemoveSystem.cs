﻿using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(RemoveRequest))]
	public sealed partial class RemoveSystem : AEntitySetSystem<float>
	{
		[WorldComponent]
		private readonly CollisionSystem _quadTree;
		
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