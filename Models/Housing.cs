using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Models
{
	public class Housing
	{
		public Housing(int totalBeds)
		{
			TotalBeds = totalBeds;
			Residents = new List<Entity>(TotalBeds);
		}

		public int TotalBeds { get; }
		public int UsedBeds => Residents.Count;
		public int EmptyBeds => TotalBeds - UsedBeds;
		public bool HasEmptyBeds => EmptyBeds > 0;
		public List<Entity> Residents { get; }
	}
}
