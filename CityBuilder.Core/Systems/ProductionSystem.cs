using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[Without(typeof(Input))]
[Without(typeof(CanNotWorkReason))]
public sealed partial class ProductionSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update, UseBuffer]
	private void Update(in Entity workplace, ref WorkProgress workProgress, in Output output)
	{
		var inventory = _inventorySystem.GetGood(workplace, output.Good);

		inventory.Set<Amount>(inventory.Get<Amount>() + output.Amount);
		workProgress -= 1;
		workplace.NotifyChanged<WorkProgress>();
	}

	[WithPredicate]
	private static bool Filter(in WorkProgress workProgress) => workProgress.Value >= 1;
}
