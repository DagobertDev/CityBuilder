using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems
{
	public sealed partial class ProductionSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter]
		private readonly IInventorySystem _inventorySystem;

		[Update] [UseBuffer]
		private void Update(in Entity workplace, [Added] [Changed] in WorkProgress work, in Output output)
		{
			if (work.Value < output.Difficulty)
			{
				return;
			}
			
			var nullableInventory = _inventorySystem.GetGood(workplace, output.Good);
			
			var inventory = nullableInventory ?? _inventorySystem.SetGood(workplace, output.Good, 0);
			
			inventory.Set<Amount>(inventory.Get<Amount>() + output.Amount);
			workplace.Set<WorkProgress>(work.Value - output.Difficulty);
		}
	}
}
