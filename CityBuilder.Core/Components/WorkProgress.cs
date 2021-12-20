namespace CityBuilder.Core.Components
{
	public readonly struct WorkProgress
	{
		public WorkProgress(float value)
		{
			Value = value;
		}
		
		public float Value { get; }
		
		public static implicit operator float(WorkProgress tiredness) => tiredness.Value;
		public static implicit operator WorkProgress(float value) => new(value);
	}
}
