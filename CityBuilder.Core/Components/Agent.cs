namespace CityBuilder.Core.Components;

public readonly record struct Agent(float Speed, AIType Type);

public enum AIType
{
	Worker, Transporter,
}
