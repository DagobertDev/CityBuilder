namespace CityBuilder.Core.ModSupport
{
	public readonly struct BlueprintInfo
	{
		public BlueprintInfo(string name, string texture)
		{
			Name = name;
			Texture = texture;
		}
		
		public string Name { get; }
		public string Texture { get; }
	}
}
