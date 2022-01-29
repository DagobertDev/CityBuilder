namespace CityBuilder.Core.Components;

public readonly record struct Agent(AIType Type);

public enum AIType
{
	Worker, Transporter,
}
