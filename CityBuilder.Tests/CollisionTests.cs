using System.Drawing;
using System.Linq;
using System.Numerics;
using CityBuilder.Components;
using CityBuilder.Systems;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class CollisionTests
	{
		private World _world;
		private CollisionSystem<SizeF> _system;

		[SetUp]
		public void Setup()
		{
			_world = new World();
			_system = new CollisionSystem<SizeF>(_world, value => new Vector2(value.Width, value.Height));
		}

		[TearDown]
		public void TearDown()
		{
			_world.Dispose();
			_system.Dispose();
		}
		
		[Test]
		public void Test_CollisionWithPoint()
		{
			var first = _world.CreateEntity();
			first.Set<Position>();
			first.Set(new SizeF(2, 2));

			_system.Update(0);

			var collisions = _system.GetEntities(new Vector2(0.5f, 0.5f));

			Assert.IsNotEmpty(collisions);
		}
		
		[Test]
		public void Test_NoCollisionWithPoint()
		{
			var first = _world.CreateEntity();
			first.Set<Position>();
			first.Set(new SizeF(2, 2));

			_system.Update(0);

			var collisions = _system.GetEntities(new Vector2(1, 1));
			
			Assert.IsEmpty(collisions);
		}
		
		[Test]
		public void Test_CollisionWithRectangle()
		{
			var first = _world.CreateEntity();
			first.Set<Position>();
			first.Set(new SizeF(2, 2));

			_system.Update(0);

			var collisions = _system.GetEntities(new HitBox(Vector2.Zero, Vector2.One, _world.CreateEntity()));

			Assert.IsNotEmpty(collisions);
		}
		
		[Test]
		public void Test_NoCollisionWithRectangle()
		{
			var first = _world.CreateEntity();
			first.Set<Position>();
			first.Set(new SizeF(2, 2));

			_system.Update(0);

			var collisions = _system.GetEntities(new HitBox(Vector2.One * 2, Vector2.One, _world.CreateEntity()));
			
			Assert.IsEmpty(collisions);
		}
	}
}
