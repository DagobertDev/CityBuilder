using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionWithInputSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update, UseBuffer]
	private void Update(in Entity workplace, ref WorkProgress workProgress, in Output output, in Input required)
	{
		var inventory = _inventorySystem.GetGood(workplace, output.Good);

		inventory.Set<Amount>(inventory.Get<Amount>() + output.Amount);

		if (inventory.Get<UnusedCapacity>() < output.Amount)
		{
			workplace.AddFlag(CanNotWorkReason.InventoryFull);
		}

		workProgress -= 1;
		workplace.NotifyChanged<WorkProgress>();

		var newAmount = new List<(Entity, int)>(required.Value.Count);

		foreach (var goodAndAmount in required.Value)
		{
			var good = goodAndAmount.Key;
			var requiredAmount = goodAndAmount.Value;

			var inputInventory = _inventorySystem.GetGood(workplace, good);
			var availableAmount = inputInventory.Get<Amount>();
			var remainingAmount = availableAmount - requiredAmount;

			if (remainingAmount < 0)
			{
				workplace.AddFlag(CanNotWorkReason.NoInput);
				return;
			}

			newAmount.Add((inputInventory, remainingAmount));
		}

		foreach (var (inputInventory, remainingAmount) in newAmount)
		{
			inputInventory.Set<Amount>(remainingAmount);
		}
	}

	[WithPredicate]
	private static bool Filter(in WorkProgress workProgress) => workProgress.Value >= 1;
}
