using CityBuilder.Core.Components.Inventory;
using DefaultEcs;

namespace CityBuilder.Core.Components;

public record Transport(Entity From, Entity To, Good Good, Amount Amount,
	TransportState State = TransportState.Fetching);

public enum TransportState
{
	Fetching,
	Loading,
	Delivering,
	Unloading,
}
