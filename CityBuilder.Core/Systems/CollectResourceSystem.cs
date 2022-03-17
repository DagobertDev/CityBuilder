using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class CollectResourceSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly ICollisionSystem _collisionSystem;

	[Update, UseBuffer]
	private void Update(in Entity entity, in CollectResource collect, in HitBox hitBox)
	{
		foreach (var resourceEntity in _collisionSystem.GetEntities(hitBox))
		{
			if (!resourceEntity.Has<Resource>())
			{
				continue;
			}

			var resource = resourceEntity.Get<Resource>();

			if (resource.Type != collect.Type)
			{
				continue;
			}

			resourceEntity.Set(resource with { CollectionStatus = Resource.Status.CollectionRequested });
		}

		entity.Dispose();
	}
}
