using System;
using System.Collections.Generic;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

[With(typeof(Agent))]
[Without(typeof(Resident))]
[With(typeof(Position))]
public class HousingSystem : AEntitySetSystem<float>
{
	private readonly EntitySet _emptyHouses;
	private readonly EntityMultiMap<Resident> _residents;

	public HousingSystem(World world) : base(world, true)
	{
		World.SubscribeComponentAdded<Resident>(AddResident);
		World.SubscribeComponentChanged<Resident>(ChangeResident);
		World.SubscribeComponentRemoved<Resident>(RemoveResident);

		World.SubscribeComponentRemoved((in Entity house, in Housing _) =>
		{
			foreach (var resident in GetResidents(house))
			{
				resident.Remove<Resident>();
			}
		});

		_emptyHouses = world.GetEntities().With((in Housing housing) => housing.HasEmptyBeds).With<Position>().AsSet();
		_residents = world.GetEntities().With<Position>().AsMultiMap<Resident>();
	}
		
	protected override void Update(float state, in Entity resident)
	{
		var house = FindBestHouse(resident, _emptyHouses.GetEntities());

		if (house.IsAlive)
		{
			resident.Set(new Resident(house));
		}
	}

	private static Entity FindBestHouse(Entity resident, ReadOnlySpan<Entity> houses)
	{
		var position = resident.Get<Position>().Value;

		Entity bestMatch = default;
		var bestDistance = float.MaxValue;

		foreach (var house in houses)
		{
			var housePosition = house.Get<Position>().Value;
			var distance = position.DistanceSquaredTo(housePosition);

			if (distance < bestDistance)
			{
				bestMatch = house;
				bestDistance = distance;
			}
		}

		return bestMatch;
	}

	private static void AddResident(in Entity entity, in Resident resident)
	{
		var house = resident.Home;
		var housing = house.Get<Housing>();
		housing = new Housing(housing.MaxBeds, housing.UsedBeds + 1);
		house.Set(housing);
	}

	private static void RemoveResident(in Entity entity, in Resident resident)
	{
		entity.Remove<IsAtHome>();

		var house = resident.Home;
		var housing = house.Get<Housing>();
		housing = new Housing(housing.MaxBeds, housing.UsedBeds - 1);
		house.Set(housing);
	}
		
	private static void ChangeResident(in Entity entity, in Resident oldResident, in Resident newResident)
	{
		RemoveResident(entity, oldResident);
		AddResident(entity, newResident);
	}

	public ICollection<Entity> GetResidents(Entity house)
	{
		if (_residents.TryGetEntities(new Resident(house), out var residents))
		{
			return residents.ToArray();
		}

		return Array.Empty<Entity>();
	}
}