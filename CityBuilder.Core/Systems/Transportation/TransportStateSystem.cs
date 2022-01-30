using System;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.Transportation;

[With(typeof(Idling))]
public sealed partial class TransportStateSystem : AEntitySetSystem<float>
{
	private const int LoadTime = 1;
	private const int UnloadTime = 1;

	[Update, UseBuffer]
	private static void Update(in Entity entity, ref Transport transport)
	{
		entity.Remove<Idling>();

		switch (transport)
		{
			case { State: TransportState.Fetching }:
				Fetch(in entity, ref transport);
				break;
			case { State: TransportState.Loading }:
				Load(in entity, ref transport);
				break;
			case { State: TransportState.Delivering }:
				Deliver(in entity, ref transport);
				break;
			case { State: TransportState.Unloading }:
				Unload(in entity, ref transport);
				break;
			default: throw new NotImplementedException();
		}
	}

	private static void Fetch(in Entity entity, ref Transport transport)
	{
		var position = entity.Get<Position>();
		var from = transport.From;

		if (!from.IsAlive)
		{
			throw new ApplicationException("Transport start point is not alive.");
		}

		var destination = from.Get<Position>();

		if (position.Value == destination.Value)
		{
			transport = transport with { State = TransportState.Loading };
			entity.Set<Waiting>(LoadTime);
		}
		else
		{
			entity.Set(new Destination(destination.Value));
		}
	}

	private static void Load(in Entity entity, ref Transport transport)
	{
		var from = transport.From;

		if (!from.IsAlive)
		{
			throw new ApplicationException("Transport start point is not alive.");
		}

		var requestedAmount = transport.Amount;
		var availableAmount = from.Get<Amount>();
		var transportedAmount = Math.Min(requestedAmount, availableAmount);

		from.Set<Amount>(availableAmount - transportedAmount);
		entity.Set(transport with { Amount = transportedAmount, State = TransportState.Delivering });

		var to = transport.To;

		if (!to.IsAlive)
		{
			throw new ApplicationException("Transport end point is not alive.");
		}

		var destination = to.Get<Position>().Value;
		entity.Set(new Destination(destination));
	}

	private static void Deliver(in Entity entity, ref Transport transport)
	{
		var to = transport.To;

		if (!to.IsAlive)
		{
			throw new ApplicationException("Transport end point is not alive.");
		}

		if (entity.Get<Position>().Value != to.Get<Position>().Value)
		{
			throw new ApplicationException("Transporter did not reach destination.");
		}

		entity.Set(transport with { State = TransportState.Unloading });
		entity.Set<Waiting>(UnloadTime);
	}

	private static void Unload(in Entity entity, ref Transport transport)
	{
		var to = transport.To;

		if (!to.IsAlive)
		{
			throw new ApplicationException("Transport end point is not alive.");
		}

		var addedAmount = transport.Amount;
		to.Get<Amount>() += addedAmount;
		to.NotifyChanged<Amount>();

		entity.Remove<Transport>();
		entity.Set<Idling>();
	}
}
