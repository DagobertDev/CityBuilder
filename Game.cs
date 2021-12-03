using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.GUI;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using CityBuilder.Systems;
using CityBuilder.Systems.UI;
using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder
{
	public class Game : Control
	{
		public static World World { get; } = new();
		public static IPublisher Publisher => World;
		private Node2D Map => GetNode<Node2D>("Map");

		private ISystem<float> _system = null!;

		public Game()
		{
			World.Subscribe(this);
		}

		public override void _Ready()
		{
			var collisionSystem = new CollisionSystem(World);
			World.SetMaxCapacity<CollisionSystem>(1);
			World.Set(collisionSystem);

			World.SubscribeComponentRemoved((in Entity _, in Sprite sprite) => sprite.QueueFree());

			_system = new SequentialSystem<float>(
				new RemoveSystem(World),
				new SpriteCreationSystem(World, Map),
				new SpritePositionSystem(World),
				new MovementSystem(World),
				collisionSystem,
				new LocationSensorSystem(World),
				new AISystem(World),
				new WaitSystem(World),
				new HungerSystem(World),
				new TirednessSystem(World),
				new HousingSystem(World),
				new SleepSystem(World),
				new WorkSystem(World),
				new WorkingSystem(World),
				new ConstructionSystem(World),
				new ConstructionProgressVisualisationInitSystem(World),
				new ConstructionProgressVisualisationSystem(World));

			var textureManager = new TextureManager();
			textureManager.Manage(World);
			
			var modLoader = new ModLoader(textureManager, ProjectSettings.GlobalizePath("res://mods"));
			modLoader.LoadMods();
		}

		public override void _Process(float delta)
		{
			_system.Update(delta);
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed(InputAction.MouseclickLeft))
			{
				var mousePosition = GetGlobalMousePosition();
				
				var selected = World.Get<CollisionSystem>().GetEntities(mousePosition).FirstOrDefault();

				if (selected.IsAlive)
				{
					World.Publish(new EntitySelected(selected));
				}
			}
		}

		[Subscribe]
		private void On(in BlueprintPlacedMessage message)
		{
			if (false)
			{
				var random = new Random();

				for (var i = 0; i < 1000; i++)
				{
					var entity = World.CreateEntity();
					var position  = new System.Numerics.Vector2(100000 * (float)random.NextDouble(), 100000 * (float)random.NextDouble());
					entity.Set(new Position(position));
				}
			}

			else
			{
				var position = new Position(message.Transform.origin.ToNumericsVector());
				var blueprint = message.Blueprint;
				
				if (message.Blueprint.Entity.Has<Construction>())
				{
					var entity = World.CreateEntity();
					entity.Set(position);
					
					entity.Set(blueprint);
					
					var construction = blueprint.Entity.Get<Construction>();
					entity.Set(construction);
				
					var texture = blueprint.Entity.Get<ManagedResource<string, Texture>>();
					entity.Set(texture);
				
					entity.Set(new Workplace(construction.Workers));
				}

				else
				{
					World.Publish(new FinishedBuilding(blueprint, position));
				}
			}
		}

		[Subscribe]
		private void On(in FinishedBuilding message)
		{
			var entity = World.CreateEntity();
			entity.Set(message.Position);
			message.Blueprint.Populate(entity);
			entity.Remove<Construction>();
		}
	}
}
