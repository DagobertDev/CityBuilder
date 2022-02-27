using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class TransportStateSystem : AEntitySetSystem<float>
{
	private const int LoadTime = 5;
	private const int UnloadTime = 5;

	[Update, UseBuffer]
	private static void Update(in Entity entity, ref Transport transport, [Changed] in BehaviorState state)
	{
		switch (state)
		{
			case Starting:
				GoToStart(in entity, in transport);
				break;
			case Fetching:
				StartLoading(in entity, in transport);
				break;
			case Loading:
				Load(in entity, ref transport);
				break;
			case Delivering:
				Deliver(in entity, ref transport);
				break;
			case Unloading:
				Unload(in entity, in transport);
				break;
			default: throw new NotImplementedException();
		}
	}

	private static void GoToStart(in Entity entity, in Transport transport)
	{
		if (transport is not { From.IsAlive: true, To.IsAlive: true })
		{
			Cancel(entity);
			return;
		}

		var from = transport.From;
		var destination = from.Get<Position>();
		entity.Set<Destination>(destination.Value);
	}

	private static void StartLoading(in Entity entity, in Transport transport)
	{
		if (transport is not { From.IsAlive: true, To.IsAlive: true })
		{
			Cancel(entity);
			return;
		}

		entity.Set<Waiting>(LoadTime);
	}

	private static void Load(in Entity entity, ref Transport transport)
	{
		if (transport is not { From.IsAlive: true, To.IsAlive: true })
		{
			Cancel(entity);
			return;
		}

		var from = transport.From;

		var requestedAmount = transport.Amount;
		var availableAmount = from.Get<Amount>();

		if (availableAmount == 0)
		{
			Cancel(entity);
			return;
		}

		var transportedAmount = Math.Min(requestedAmount, availableAmount);

		from.Set<Amount>(availableAmount - transportedAmount);
		entity.Set(transport with { Amount = transportedAmount });

		var to = transport.To;

		var destination = to.Get<Position>().Value;
		entity.Set(new Destination(destination));
	}

	private static void Deliver(in Entity entity, ref Transport transport)
	{
		var to = transport.To;

		if (!to.IsAlive)
		{
			Cancel(entity);
			return;
		}

		if (entity.Get<Position>().Value != to.Get<Position>().Value)
		{
			throw new ApplicationException("Transporter did not reach destination.");
		}

		entity.Set<Waiting>(UnloadTime);
	}

	private static void Unload(in Entity entity, in Transport transport)
	{
		var to = transport.To;

		if (!to.IsAlive)
		{
			Cancel(entity);
			return;
		}

		ref var currentAmount = ref to.Get<Amount>();
		var addedAmount = Math.Min(transport.Amount, to.Get<Capacity>() - currentAmount);
		currentAmount += addedAmount;
		to.NotifyChanged<Amount>();

		Cancel(entity);
	}

	public static void Cancel(Entity entity)
	{
		entity.Remove<Transport>();
		entity.Set(BehaviorState.Deciding);
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Fetching = Starting + 1;
	private const int Loading = Fetching + 1;
	private const int Delivering = Loading + 1;
	private const int Unloading = Delivering + 1;
}
