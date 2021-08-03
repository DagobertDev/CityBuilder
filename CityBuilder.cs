using System;
using CityBuilder.Components;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using CityBuilder.Systems;
using CityBuilder.Systems.GodotInterface;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;
using World = DefaultEcs.World;

namespace CityBuilder
{
	public class CityBuilder : Node
	{
		private static World World { get; } = new();
		public static IPublisher Publisher => World;
		private Node2D Map => GetNode<Node2D>("Map");

		private ISystem<float> _system = null!;

		public CityBuilder()
		{
			World.Subscribe(this);
		}

		public override void _Ready()
		{
			World.Set( new QuadTree<HitBox>(100000, 100000, new HitBoxBounds()));
			
			_system = new SequentialSystem<float>(
				new SpriteCreationSystem(World, Map),
				new SpritePositionSystem(World),
				new MovementSystem(World),
				new QuadTreeSystem(World),
				new LocationSensorSystem(World),
				new RemoveOldLocationSystem(World),
				new AISystem(World),
				new WaitSystem(World),
				new TirednessSystem(World),
				new HousingSystem(World),
				new SleepSystem(World),
				new WorkSystem(World));

			var textureManager = new TextureManager();
			textureManager.Manage(World);
			
			var modLoader = new ModLoader(textureManager);
			modLoader.LoadMods();
		}

		public override void _Process(float delta)
		{
			_system.Update(delta);
		}

		[Subscribe]
		private void On(in BlueprintPlacedMessage message)
		{
			if (false)
			{
				var random = new Random();

				for (var i = 0; i < 1000; i++)
				{
					var transform = Transform2D.Identity;
					transform.origin = new Vector2(100000 * (float)random.NextDouble(), 100000 * (float)random.NextDouble());

					var entity = World.CreateEntity();

					message.Blueprint.Populate(entity);

					entity.Set(transform);
				}
			}

			else
			{
				var entity = World.CreateEntity();

				message.Blueprint.Populate(entity);

				entity.Set(message.Transform);
			}
		}
	}
}
