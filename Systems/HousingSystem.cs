using System;
using CityBuilder.Models;
using CityBuilder.Models.Flags;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(SearchingHome))]
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
			foreach (var home in _emptyHouses.GetEntities())
			{
				AddHome(resident, home);
				break;
			}
		}

		private static void AddHome(Entity resident, Entity home)
		{
			if (resident.Has<Resident>())
			{
				throw new Exception("Resident already has home");
				// TODO: Set new home if resident already has one
				/*var oldHome = resident.Get<Resident>().Home;
				var oldHousing = oldHome.Get<Housing>();
				oldHousing.Residents.Remove(resident);

				if (oldHousing.HasEmptyBeds)
				{
					oldHome.Set<EmptyHousing>();
				}*/
			}

			resident.Set(new Resident(home));
			resident.Remove<SearchingHome>();

			var housing = home.Get<Housing>();

			housing.Residents.Add(resident);

			if (!housing.HasEmptyBeds)
			{
				home.Remove<EmptyHousing>();
			}
		}
	}
}
