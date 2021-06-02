using System;
using CityBuilder.Models;
using CityBuilder.Models.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(Agent))]
	[Without(typeof(Resident))]
	[With(typeof(Transform2D))]
	public class HousingSystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _emptyHouses;

		public HousingSystem(World world) : base(world, true)
		{
			_emptyHouses = world.GetEntities().With<Housing>().With<EmptyHousing>().With<Transform2D>().AsSet();
		}

		protected override void Update(float state, in Entity resident)
		{
			var house = FindBestHouse(resident, _emptyHouses.GetEntities());

			if (house.IsAlive)
			{
				SetHouse(resident, house);
			}
		}

		private static Entity FindBestHouse(Entity resident, ReadOnlySpan<Entity> houses)
		{
			var position = resident.Get<Transform2D>().origin;

			Entity bestMatch = default;
			var bestDistance = float.MaxValue;

			foreach (var house in houses)
			{
				var housePosition = house.Get<Transform2D>().origin;
				var distance = position.DistanceSquaredTo(housePosition);

				if (distance < bestDistance)
				{
					bestMatch = house;
					bestDistance = distance;
				}
			}

			return bestMatch;
		}

		private static void SetHouse(Entity resident, Entity home)
		{
			if (resident.Has<Resident>())
			{
				throw new NotImplementedException("Resident already has home");
			}

			resident.Set(new Resident(home));

			var housing = home.Get<Housing>();

			housing.Residents.Add(resident);

			if (!housing.HasEmptyBeds)
			{
				home.Remove<EmptyHousing>();
			}
		}
	}
}
