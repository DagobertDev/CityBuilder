﻿using System;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using CityBuilder.Systems.GodotInterface;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public class System
	{
		private readonly World _world;

		private System(World world)
		{
			_world = world;
		}

		public static ISystem<float> Create(World world)
		{
			world.Subscribe(new System(world));
			world.Subscribe(new ModLoader());

			TextureManager.Instance.Manage(world);

			return new SequentialSystem<float>(
				new SpriteCreationSystem(world),
				new SpritePositionSystem(world),
				new MovementSystem(world),
				new LocationSensorSystem(world),
				new RemoveOldLocationSystem(world),
				new AISystem(world),
				new TirednessSystem(world),
				new HousingSystem(world),
				new SleepSystem(world),
				new WorkSystem(world));
		}

		[Subscribe]
		private void On(in BlueprintPlacedMessage message)
		{
			if (true)
			{
				var random = new Random();

				for (var i = 0; i < 1000; i++)
				{
					var transform = Transform2D.Identity;
					transform.origin = new Vector2(100000 * (float)random.NextDouble(), 100000 * (float)random.NextDouble());

					var entity = _world.CreateEntity();

					message.Blueprint.Populate(entity);

					entity.Set(transform);
				}
			}

			else
			{
				var entity = _world.CreateEntity();

				message.Blueprint.Populate(entity);

				entity.Set(message.Transform);
			}
		}
	}
}
