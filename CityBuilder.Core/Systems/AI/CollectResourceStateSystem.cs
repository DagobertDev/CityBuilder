using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class CollectResourceStateSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly IInventorySystem _inventorySystem;

	[Update, UseBuffer]
	private void Update(in Entity entity, in CollectingResource collecting, [Changed] BehaviorState state)
	{
		if (!collecting.Resource.IsAlive)
		{
			entity.Remove<CollectingResource>();
			entity.Set(BehaviorState.Deciding);
			return;
		}

		switch (state)
		{
			case Starting:
				entity.Set<Destination>(collecting.Resource.Get<Position>().Value);
				break;
			case Collecting:
				entity.Set<Waiting>(collecting.Resource.Get<Resource>().Difficulty);
				break;
			case Finished:
				var resourceEntity = collecting.Resource;
				var (good, amount) = resourceEntity.Get<Output>();
				var position = resourceEntity.Get<Position>();
				resourceEntity.Dispose();

				var pile = _inventorySystem.CreatePile(position, good, amount);

				if (collecting.TransportTo.IsAlive)
				{
					_inventorySystem.EnsureCreated(collecting.TransportTo, good);
					var workInventory = _inventorySystem.GetGood(collecting.TransportTo, good);

					var transportedAmount = Math.Min(amount, workInventory.Get<FutureUnusedCapacity>());
					entity.Set(new Transport(pile, workInventory, new Good(good), transportedAmount));
					entity.Remove<CollectingResource>();
					entity.Set(BehaviorState.Starting);
				}
				else
				{
					entity.Remove<CollectingResource>();
					entity.Set(BehaviorState.Deciding);
				}

				break;
		}
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Collecting = Starting + 1;
	private const int Finished = Collecting + 1;
}
