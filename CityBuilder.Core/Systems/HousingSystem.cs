using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	[With(typeof(Agent))]
	[Without(typeof(Resident))]
	[With(typeof(Position))]
	public class HousingSystem : AEntitySetSystem<float>
	{
		private readonly EntitySet _emptyHouses;

		public HousingSystem(World world) : base(world, true)
		{
			world.SubscribeComponentAdded<Housing>(Initialize);
			
			World.SubscribeComponentRemoved<Resident>(RemoveResident);

			World.SubscribeComponentRemoved((in Entity _, in Housing housing) =>
			{
				foreach (var employee in housing.Residents.Where(entity => entity.IsAlive).ToList())
				{
					employee.Remove<Resident>();
				}
			});

			_emptyHouses = world.GetEntities().With<Housing>().With<EmptyHousing>().With<Position>().AsSet();
		}

		private static void Initialize(in Entity entity, in Housing housing)
		{
			if (housing.HasEmptyBeds)
			{
				entity.Set<EmptyHousing>();
			}

			else
			{
				entity.Remove<EmptyHousing>();
			}
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

		public static void RemoveResident(in Entity entity, in Resident resident)
		{
			entity.Remove<IsAtHome>();

			var home = resident.Home;

			if (home.IsAlive)
			{
				home.Get<Housing>().Residents.Remove(entity);
				home.NotifyChanged<Housing>();
			}
		}
	}
}
