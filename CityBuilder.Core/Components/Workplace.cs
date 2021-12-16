namespace CityBuilder.Components
{
	public class Workplace
	{
		public Workplace(int maxEmployees, int currentEmployees = 0)
		{
			MaxEmployees = maxEmployees;
			CurrentEmployees = currentEmployees;
		}

		public int MaxEmployees { get; }
		public int CurrentEmployees { get; }
		public int EmptyWorkspace => MaxEmployees - CurrentEmployees;
		public bool HasEmptyWorkspace => CurrentEmployees < MaxEmployees;
	}
}
