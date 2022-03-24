using DefaultEcs;

namespace CityBuilder.Core.Components.AI;

public readonly record struct CollectingResource(Entity Resource, Entity TransportTo = default);
