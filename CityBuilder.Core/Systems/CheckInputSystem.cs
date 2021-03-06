using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public sealed partial class CheckInputSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update, UseBuffer]
	private void Update(in Entity entity, in Input input)
	{
		foreach (var goodAndAmount in input.Value)
		{
			var good = goodAndAmount.Key;
			var requiredAmount = goodAndAmount.Value;

			var inventory = _inventorySystem.GetGood(entity, good);

			if (inventory.Get<Amount>() < requiredAmount)
			{
				return;
			}
		}

		entity.RemoveFlag(CanNotWorkReason.NoInput);
	}

	[WithPredicate]
	private static bool Filter(in CanNotWorkReason reason) => reason.HasFlag(CanNotWorkReason.NoInput);
}
