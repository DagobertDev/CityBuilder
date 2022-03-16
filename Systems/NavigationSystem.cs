using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	[With(typeof(Destination))]
	[Without(typeof(Waypoint))]
	public sealed partial class NavigationSystem : AEntitySetSystem<float>
	{
		[Update, UseBuffer]
		private static void Update(in Entity entity, in NavigationAgent2D agent, in Destination destination,
			in Position position)
		{
			// TODO: Fix this
			/*if (agent.IsNavigationFinished())
			{
				entity.Remove<Destination>();
				entity.Get<BehaviorState>().Next(out var next);
				entity.Set(next);
				return;
			}

			var waypoint = agent.GetNextLocation().ToNumericsVector();
			entity.Set<Waypoint>(waypoint);*/

			if (destination.Position == position.Value)
			{
				entity.Remove<Destination>();
				entity.Get<BehaviorState>().Next(out var next);
				entity.Set(next);
				return;
			}

			entity.Set<Waypoint>(destination.Position);
		}
	}
}
