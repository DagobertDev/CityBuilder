using CityBuilder.Systems;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder
{
	public class CityBuilder : Node
	{
		private static World World { get; } = new();
		public static IPublisher Publisher => World;
		public static CityBuilder Instance { get; private set; }
		public Node2D Map => GetNode<Node2D>("Map");

		private ISystem<float> _system;

		public CityBuilder()
		{
			Instance = this;
		}

		public override void _Ready()
		{
			_system = Systems.System.Create(World);
			World.Publish(new LoadMods());
		}

		public override void _Process(float delta)
		{
			_system.Update(delta);
		}
	}
}
