using CityBuilder.Components;
using CityBuilder.Core.Components;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems
{
	public sealed partial class NavigationDestinationSystem : AEntitySetSystem<float>
	{
		[Update]
		private static void Update([Added, Changed] Destination destination, NavigationAgent2D agent)
		{
			agent.SetTargetLocation(destination.Position.ToGodotVector());
		}
	}
}
