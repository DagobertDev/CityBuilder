using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using CityBuilder.Systems;
using CityBuilder.Systems.GodotInterface;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using UltimateQuadTree;
using InputMap = CityBuilder.GUI.InputMap;
using World = DefaultEcs.World;

namespace CityBuilder
{
	public class Game : Control
	{
		private static World World { get; } = new();
		public static IPublisher Publisher => World;
		private Node2D Map => GetNode<Node2D>("Map");

		private ISystem<float> _system = null!;

		public Game()
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
				new HungerSystem(World),
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

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed(InputMap.MouseclickLeft))
			{
				var mousePosition = GetGlobalMousePosition();
				
				var selected = World.Get<QuadTree<HitBox>>()
					.GetNearestObjects(new HitBox(mousePosition, Vector2.One, default));

				var first = selected.FirstOrDefault(hitBox => hitBox.Value.HasPoint(mousePosition));

				if (first is not null)
				{
					World.Publish(new EntitySelected(first.Entity));
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

					message.Blueprint.Populate(entity);

					var position  = new System.Numerics.Vector2(100000 * (float)random.NextDouble(), 100000 * (float)random.NextDouble());
					entity.Set(new Position(position));
				}
			}

			else
			{
				var entity = World.CreateEntity();

				message.Blueprint.Populate(entity);

				var position = new System.Numerics.Vector2(message.Transform.origin.x, message.Transform.origin.y);

				entity.Set(new Position(position));
			}
		}
	}
}
