﻿using System.Numerics;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems.AI;

public sealed class AISystem : ISystem<float>
{
	private readonly ISystem<float> _system;

	public AISystem(World world, Vector2 mapSize, IInventorySystem inventorySystem)
	{
		_system = new SequentialSystem<float>
		(
			// Decision Systems
			new EatDecisionSystem(world),
			new SleepDecisionSystem(world),
			new WorkingDecisionSystem(world),
			new CollectResourceAsWorkDecisionSystem(world),
			new CollectResourceDecisionSystem(world),
			new TransportDecisionSystem(world),
			new WanderAroundDecisionSystem(world),
			new DoNothingDecisionSystem(world),

			// Validation
			new ValidateWorkingDecisionSystem(world),

			// State Systems
			new EatStateSystem(world),
			new SleepStateSystem(world),
			new WorkingStateSystem(world),
			new CollectResourceStateSystem(world, inventorySystem),
			new TransportStateSystem(world),
			new WanderAroundStateSystem(world, mapSize),
			new DoNothingStateSystem(world)
		);
	}

	public void Dispose() => _system.Dispose();

	public void Update(float state) => _system.Update(state);

	public bool IsEnabled
	{
		get => _system.IsEnabled;
		set => _system.IsEnabled = value;
	}
}
