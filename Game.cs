using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.GUI;
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
			var quadTree = new QuadTree<HitBox>(-10000, -10000, 110000, 110000, new HitBoxBounds());
			World.Set(quadTree);
			
			World.SubscribeComponentRemoved((in Entity _, in HitBox hitBox) => quadTree.Remove(hitBox));
			
			World.SubscribeComponentRemoved((in Entity _, in Sprite sprite) => sprite.QueueFree());
			
			World.SubscribeComponentRemoved((in Entity entity, in Employee employee) =>
			{
				var workplace = employee.Workplace;

				if (workplace.IsAlive)
				{
					workplace.Get<Workplace>().Employees.Remove(entity);
					workplace.NotifyChanged<Workplace>();
				}
			});

			World.SubscribeComponentRemoved((in Entity _, in Workplace workplace) =>
			{
				foreach (var employee in workplace.Employees.Where(entity => entity.IsAlive).ToList())
				{
					employee.Remove<Employee>();
				}
			});
			
			World.SubscribeComponentRemoved((in Entity entity, in Resident resident) =>
			{
				var home = resident.Home;

				if (home.IsAlive)
				{
					home.Get<Housing>().Residents.Remove(entity);
					home.NotifyChanged<Housing>();
				}
			});

			World.SubscribeComponentRemoved((in Entity _, in Housing housing) =>
			{
				foreach (var employee in housing.Residents.Where(entity => entity.IsAlive).ToList())
				{
					employee.Remove<Resident>();
				}
			});

			_system = new SequentialSystem<float>(
				new RemoveSystem(World),
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
				new HousingInitSystem(World),
				new SleepSystem(World),
				new WorkSystem(World),
				new WorkspaceInitSystem(World));

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
