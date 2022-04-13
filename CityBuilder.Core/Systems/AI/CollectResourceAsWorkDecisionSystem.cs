using System;
using System.Collections.Generic;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class CollectResourceAsWorkDecisionSystem : AEntitySetSystem<float>
{
	private readonly EntityMultiMap<Resource> _resources;
	private readonly EntityMultiMap<Resource> _grownResources;

	public CollectResourceAsWorkDecisionSystem(World world) : base(world, CreateEntityContainer, true)
	{
		_resources = world.GetEntities()
			.With((in Resource resource) => resource.CollectionStatus != Resource.Status.Reserved)
			.Without<Growth>()
			.AsMultiMap(new ResourceTypeComparer());

		_grownResources = world.GetEntities()
			.With((in Resource resource) => resource.CollectionStatus != Resource.Status.Reserved)
			.With((in Growth growth) => growth >= 1)
			.AsMultiMap(new ResourceTypeComparer());
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, in Employee employee)
	{
		var workplace = employee.Workplace;
		var workplacePosition = employee.Location;

		if (!workplace.Has<ResourceCollector>() || workplace.Has<CanNotWorkReason>())
		{
			return;
		}

		var resourceType = employee.Workplace.Get<ResourceCollector>().Type;

		_resources.TryGetEntities(new Resource { Type = resourceType }, out var resources);
		_grownResources.TryGetEntities(new Resource { Type = resourceType }, out var grownResources);

		var resource = FindBestResource(workplacePosition, resources, grownResources);

		if (resource == default)
		{
			return;
		}

		resource.Set(resource.Get<Resource>() with { CollectionStatus = Resource.Status.Reserved });
		entity.Set(new CollectingResource(resource, workplace));
		entity.Set(BehaviorState.Starting);
	}

	[WithPredicate]
	private static bool Filter(in BehaviorState state) => state.HasNotDecided;

	private static Entity FindBestResource(Vector2 position, ReadOnlySpan<Entity> resources,
		ReadOnlySpan<Entity> grownResources)
	{
		var distance = float.MaxValue;
		Entity closest = default;

		FindBestResourceInternal(resources);
		FindBestResourceInternal(grownResources);

		return closest;

		void FindBestResourceInternal(ReadOnlySpan<Entity> res)
		{
			foreach (var resource in res)
			{
				var newDistance = resource.Get<Position>().Value.DistanceSquaredTo(position);

				if (newDistance < distance)
				{
					distance = newDistance;
					closest = resource;
				}
			}
		}
	}

	private class ResourceTypeComparer : IEqualityComparer<Resource>
	{
		public bool Equals(Resource x, Resource y) => x.Type == y.Type;

		public int GetHashCode(Resource obj) => obj.Type.GetHashCode();
	}
}
