using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class ProductionSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update] [UseBuffer]
	private void Update(in Entity workplace, ref WorkProgress workProgress, in Output output)
	{
		var nullableInventory = _inventorySystem.GetGood(workplace, output.Good);
			
		var inventory = nullableInventory ?? _inventorySystem.SetGood(workplace, output.Good, 0);
			
		inventory.Set<Amount>(inventory.Get<Amount>() + output.Amount);
		workProgress -= 1;
		workplace.NotifyChanged<WorkProgress>();

		if (!workplace.Has<Input>())
		{
			return;
		}

		var required = workplace.Get<Input>();
		var inputInventory = _inventorySystem.GetGood(workplace, required.Good) ?? _inventorySystem.SetGood(workplace, required.Good, 0);
		var availableAmount = inputInventory.Get<Amount>();
		inputInventory.Set<Amount>(Math.Max(0, availableAmount - required.Amount));
	}

	[WithPredicate]
	private static bool Filter(in WorkProgress workProgress) => workProgress.Value >= 1;
}
