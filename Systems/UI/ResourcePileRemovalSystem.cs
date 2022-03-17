using CityBuilder.Core.Components.Inventory;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems.UI
{
	[With(typeof(ResourcePile))]
	public sealed partial class ResourcePileRemovalSystem : AEntitySetSystem<float>
	{
		[Update, UseBuffer]
		private static void Update(in Entity entity)
		{
			entity.Dispose();
		}

		[WithPredicate]
		private static bool Filter(in Amount amount) => amount <= 0;
	}
}
