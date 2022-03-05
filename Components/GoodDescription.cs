using Godot;

namespace CityBuilder.Components
{
	public record GoodDescription(string Id, Texture Icon)
	{
		public string Name { get; set; } = Id;
	}
}
