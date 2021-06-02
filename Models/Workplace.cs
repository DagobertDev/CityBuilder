using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Models
{
	public class Workplace
	{
		public Workplace(int totalWorkspace)
		{
			TotalWorkspace = totalWorkspace;
			Employees = new List<Entity>(TotalWorkspace);
		}

		public int TotalWorkspace { get; }
		public int UsedWorkspace => Employees.Count;
		public int EmptyWorkspace => TotalWorkspace - UsedWorkspace;
		public bool HasEmptyWorkspace => EmptyWorkspace > 0;
		public List<Entity> Employees { get; }
	}
}
