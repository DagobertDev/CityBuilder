using CityBuilder.Components;
using CityBuilder.ModSupport;
using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	[Without(typeof(Construction))]
	public sealed partial class BlueprintSystem : AEntitySetSystem<float>
	{
		[Update] [UseBuffer]
		private static void Update(in Entity entity, in Blueprint blueprint)
		{
			if (blueprint.Entity.Has<Construction>())
			{
				var construction = blueprint.Entity.Get<Construction>();
				entity.Set(construction);
				
				var texture = blueprint.Entity.Get<ManagedResource<string, Texture>>();
				entity.Set(texture);
				
				entity.Set(new Workplace(construction.Workers));
			}

			else
			{
				blueprint.Populate(entity);
				
				entity.Remove<Blueprint>();
			}
		}
	}
}
