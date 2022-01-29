using CityBuilder.Core.Components.Inventory;
using DefaultEcs;

namespace CityBuilder.Core.Components;

public record Transport(Entity From, Entity To, Good Good, Amount Amount, bool Delivering = false);
