using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.UI
{
	[With(typeof(ResourcePile))]
	[Without(typeof(Sprite))]
	public sealed partial class ResourcePileVisualisationSystem : AEntitySetSystem<float>
	{
		[Update, UseBuffer]
		private static void Update(in Entity entity)
		{
			entity.Set(Global.GoodDescriptions[entity.Get<Good>().Name].Icon);
		}
	}
}
