﻿using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Needs;
using CityBuilder.Core.Components.Production;
using CityBuilder.Core.Messages;
using CityBuilder.Core.Systems;
using CityBuilder.Core.Systems.AI;
using CityBuilder.GUI;
using CityBuilder.ModSupport;
using CityBuilder.Systems.UI;
using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.System;
using Godot;
using Input = CityBuilder.Core.Components.Production.Input;
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
			var collisionSystem = new CollisionSystem<Sprite>(World,
				-10000, -10000, 110000, 110000,
				sprite =>
				{
					var size = sprite.Texture.GetSize();

					if (Math.Abs(sprite.RotationDegrees - 90) < 1)
					{
						size = new Vector2(size.y, size.x);
					}

					return size.ToNumericsVector();
				});
			World.SetMaxCapacity<ICollisionSystem>(1);
			World.Set<ICollisionSystem>(collisionSystem);

			var inventorySystem = new InventorySystem(World);
			World.SetMaxCapacity<IInventorySystem>(1);
			World.Set<IInventorySystem>(inventorySystem);

			World.SubscribeComponentRemoved((in Entity _, in Sprite sprite) => sprite.QueueFree());

			World.SubscribeComponentAdded((in Entity entity, in Market _) =>
			{
				if (!entity.Has<Good>())
				{
					inventorySystem.EnsureCreated(entity, Goods.Food);
				}
			});

			World.SubscribeComponentAdded((in Entity entity, in Agent _) => entity.Set(BehaviorState.Deciding));

			World.SubscribeComponentAdded((in Entity entity, in HungerRate _) => entity.Set<Hunger>());
			World.SubscribeComponentAdded((in Entity entity, in TirednessRate _) => entity.Set<Tiredness>());

			World.SubscribeComponentAdded((in Entity entity, in Input input) =>
			{
				foreach (var good in input.Value.Keys)
				{
					inventorySystem.EnsureCreated(entity, good);
				}

				if (entity.Has<CanNotWorkReason>())
				{
					entity.Set(entity.Get<CanNotWorkReason>() | CanNotWorkReason.NoInput);
				}
				else
				{
					entity.Set(CanNotWorkReason.NoInput);
				}
			});

			World.SubscribeComponentAdded((in Entity entity, in Output output)
				=> inventorySystem.EnsureCreated(entity, output.Good));

			_system = new SequentialSystem<float>(
				new RemoveSystem(World, collisionSystem),
				new SpriteCreationSystem(World, Map),
				new SpritePositionSystem(World),
				new MovementSystem(World),
				collisionSystem,
				new LocationSensorSystem(World),
				new AISystem(World),
				new TransportReservationSystem(World),
				new WaitingSystem(World),
				new HungerSystem(World),
				new TirednessSystem(World),
				new HousingSystem(World),
				new SleepSystem(World, 10f),
				new WorkSystem(World),
				new WorkingSystem(World),
				new ProductionWithInputSystem(World, inventorySystem),
				new ProductionSystem(World, inventorySystem),
				new CheckInputSystem(World, inventorySystem),
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

				var selected = World.Get<ICollisionSystem>().GetEntities(mousePosition.ToNumericsVector())
					.FirstOrDefault();

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
					var position = new System.Numerics.Vector2(100000 * (float)random.NextDouble(),
						100000 * (float)random.NextDouble());
					entity.Set(new Position(position));
				}
			}

			else
			{
				var position = message.Position;
				var blueprint = message.Blueprint;
				var rotation = message.Rotation;

				if (message.Blueprint.Entity.Has<ConstructionReference>())
				{
					var construction = message.Blueprint.Entity.Get<ConstructionReference>().Value;
					var entity = World.CreateEntity();
					entity.Set(position);

					new ComponentCloner().Clone(construction, entity);
					entity.Set(rotation);

					entity.Set(blueprint);

					var texture = blueprint.Entity.Get<ManagedResource<string, Texture>>();
					entity.Set(texture);
				}

				else
				{
					World.Publish(new FinishedBuilding(blueprint, position, rotation));
				}
			}
		}

		[Subscribe]
		private void On(in FinishedBuilding message)
		{
			var entity = World.CreateEntity();
			entity.Set(message.Position);
			entity.Set(message.Rotation);
			message.Blueprint.Populate(entity);
			entity.Remove<Construction>();
		}
	}
}
