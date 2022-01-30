using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

[With(typeof(Idling))]
public sealed partial class WanderAroundSystem : AEntitySetSystem<float>
{
	private const float DownTime = 3;
	private const int WalkDistance = 500;

	private readonly Random _random = new();

	[Update, UseBuffer]
	private void Update(in Entity entity, in Position position, in BehaviorQueue behaviorQueue)
	{
		var offset = new Vector2(
			_random.Next(-WalkDistance, WalkDistance),
			_random.Next(-WalkDistance, WalkDistance));

		var destination = position + offset;
		entity.Set<Destination>(destination);
		behaviorQueue.Enqueue(Wait);
	}

	[WithPredicate]
	private static bool Filter(in Agent agent) => agent.Type == AIType.Worker;

	private static void Wait(Entity entity) => entity.Set<Waiting>(DownTime);
}
