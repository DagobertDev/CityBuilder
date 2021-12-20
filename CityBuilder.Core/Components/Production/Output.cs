namespace CityBuilder.Core.Components.Production
{
	public readonly struct Output
	{
		public Output(string good, int amount, int difficulty)
		{
			Good = good;
			Amount = amount;
			Difficulty = difficulty;
		}
		
		public string Good { get; }
		public int Amount { get; }
		public int Difficulty { get; }
	}
}
