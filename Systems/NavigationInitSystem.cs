using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems;

[Without(typeof(NavigationAgent2D))]
public sealed partial class NavigationInitSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, in Speed speed, Sprite sprite)
	{
		var agent = new NavigationAgent2D
		{
			MaxSpeed = speed,
		};

		entity.Set(agent);
		sprite.AddChild(agent);
	}
}
