using CityBuilder.Models;

namespace CityBuilder.Messages
{
	public readonly struct BlueprintSelectedMessage
	{
		public BlueprintSelectedMessage(Blueprint blueprint)
		{
			Blueprint = blueprint;
		}

		public Blueprint Blueprint { get; }
	}
}
