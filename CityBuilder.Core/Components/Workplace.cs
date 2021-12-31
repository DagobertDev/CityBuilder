namespace CityBuilder.Core.Components;

public readonly record struct Workplace(int MaxEmployees, float Difficulty, int CurrentEmployees = 0)
{
	public int EmptyWorkspace => MaxEmployees - CurrentEmployees;
	public bool HasEmptyWorkspace => CurrentEmployees < MaxEmployees;
}
