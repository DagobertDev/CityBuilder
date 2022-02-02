namespace CityBuilder.Core.Components.AI;

public readonly record struct BehaviorState(int Value)
{
	public bool HasNotDecided => this == Deciding;

	public void Next(out BehaviorState next) => next = Value + 1;

	public static implicit operator int(BehaviorState state) => state.Value;
	public static implicit operator BehaviorState(int value) => new(value);

	public static readonly BehaviorState Deciding = -1;
	public static readonly BehaviorState Starting = StartingValue;
	public const int StartingValue = 0;
}
