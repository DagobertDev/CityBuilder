using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class InventoryFullSystem : AEntitySetSystem<float>
{
	[Update]
	private static void Update([Added, Changed] UnusedCapacity unusedCapacity, Capacity capacity, Owner ownerComponent)
	{
		var owner = ownerComponent.Value;

		if (owner.Has<Output>())
		{
			if (unusedCapacity < owner.Get<Output>().Amount)
			{
				owner.AddFlag(CanNotWorkReason.InventoryFull);
			}
			else
			{
				owner.RemoveFlag(CanNotWorkReason.InventoryFull);
			}
		}
		else
		{
			// Keep 20% buffer.
			if (unusedCapacity < capacity / 5)
			{
				owner.AddFlag(CanNotWorkReason.InventoryFull);
			}
			else
			{
				owner.RemoveFlag(CanNotWorkReason.InventoryFull);
			}
		}
	}

	[WithPredicate]
	private static bool Filter(in InventoryType inventoryType) => inventoryType == InventoryType.Supply;
}
