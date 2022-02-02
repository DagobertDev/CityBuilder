using System;
using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Behaviors;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed partial class WanderAroundStateSystem : AEntitySetSystem<float>
{
	private const float DownTime = 3;
	private const int WalkDistance = 500;

	private readonly Random _random = new();

	[Update, UseBuffer]
	private void Update(in Entity entity, [Changed] in BehaviorState state)
	{
		switch (state)
		{
			case Starting:
				GoToRandomLocation(entity);
				break;
			case Arrived:
				Wait(entity);
				break;
			case Finished:
				OnFinished(entity);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(state));
		}
	}

	[WithPredicate]
	private static bool Filter(in Behavior behavior) => behavior == Behavior.WanderingAround;

	private void GoToRandomLocation(Entity entity)
	{
		var position = entity.Get<Position>();

		var offset = new Vector2
		(
			_random.Next(-WalkDistance, WalkDistance),
			_random.Next(-WalkDistance, WalkDistance)
		);

		var destination = position + offset;
		entity.Set<Destination>(destination);
	}

	private static void Wait(Entity entity)
	{
		entity.Set<Waiting>(DownTime);
	}

	private static void OnFinished(Entity entity)
	{
		entity.Set(BehaviorState.Deciding);
		entity.Remove<Behavior>();
	}

	private const int Starting = BehaviorState.StartingValue;
	private const int Arrived = Starting + 1;
	private const int Finished = Arrived + 1;
}
