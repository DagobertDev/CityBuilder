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
		public static CityBuilder Root { get; private set; }

		private readonly ISystem<float> _system;

		public CityBuilder()
		{
			Root = this;
			_system = Systems.System.Create(World);
		}

		public override void _Ready()
		{
			World.Publish(new LoadMods());
		}

		public override void _Process(float delta)
		{
			_system.Update(delta);
		}
	}
}
