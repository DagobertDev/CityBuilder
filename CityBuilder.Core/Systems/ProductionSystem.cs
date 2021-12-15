using System;
using CityBuilder.Components;
using CityBuilder.Components.Inventory;
using CityBuilder.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public sealed partial class ProductionSystem : AEntitySetSystem<float>
	{
		private readonly IInventorySystem _inventorySystem;

		public ProductionSystem(World world) : base(world, CreateEntityContainer, true)
		{
			_inventorySystem = World.Get<IInventorySystem>();
			
			world.SubscribeComponentAdded((in Entity workplace, in Output output) =>
			{
				if (!_inventorySystem.GetGood(workplace, output.Good).HasValue)
				{
					_inventorySystem.SetGood(workplace, output.Good, 0);
				}
			});
		}

		[Update] [UseBuffer]
		private void Update(in Entity workplace, [Added] [Changed] in WorkProgress work, in Output output)
		{
			if (work.Value < output.Difficulty)
			{
				return;
			}
			
			var nullableInventory = _inventorySystem.GetGood(workplace, output.Good);

			if (!nullableInventory.HasValue)
			{
				throw new ApplicationException("Inventory does not exist.");
			}
			
			var inventory = nullableInventory.Value;
			inventory.Set(new Amount(inventory.Get<Amount>().Value + output.Amount));
			workplace.Set<WorkProgress>(work.Value - output.Difficulty);
		}
	}
}
