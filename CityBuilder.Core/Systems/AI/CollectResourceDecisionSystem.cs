using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class CollectResourceDecisionSystem : AEntitySetSystem<float>
{
	private readonly EntitySet _resources;

	public CollectResourceDecisionSystem(World world) : base(world, CreateEntityContainer, true)
	{
		_resources = world.GetEntities()
			.With((in Resource resource) => resource.CollectionStatus == Resource.Status.CollectionRequested).AsSet();
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, in Position position)
	{
		var resources = _resources.GetEntities();

		if (resources.Length == 0)
		{
			return;
		}

		var resource = FindBestResource(position, resources);

		resource.Set(resource.Get<Resource>() with { CollectionStatus = Resource.Status.Reserved });
		entity.Set(new CollectingResource(resource));
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;

	private static Entity FindBestResource(Vector2 position, ReadOnlySpan<Entity> resources)
	{
		var distance = float.MaxValue;
		var closest = default(Entity);

		foreach (var resource in resources)
		{
			var newDistance = resource.Get<Position>().Value.DistanceSquaredTo(position);

			if (newDistance < distance)
			{
				distance = newDistance;
				closest = resource;
			}
		}

		if (!closest.IsAlive)
		{
			throw new ApplicationException("Resource not found");
		}

		return closest;
	}
}
