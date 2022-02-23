using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class CheckOutputSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update, UseBuffer]
	private void Update(in Entity entity, in Output output)
	{
		var inventory = _inventorySystem.GetGood(entity, output.Good);

		if (inventory.Get<UnusedCapacity>() >= output.Amount)
		{
			entity.RemoveFlag(CanNotWorkReason.InventoryFull);
		}
	}

	[WithPredicate]
	private static bool Filter(in CanNotWorkReason reason) => reason.HasFlag(CanNotWorkReason.InventoryFull);
}
