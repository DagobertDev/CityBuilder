using CityBuilder.Components;
using CityBuilder.Systems;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class HousingTests
	{
		private World _world;
		private ISystem<float> _system;

		[SetUp]
		public void Setup()
		{
			_world = new World();
			_system = new HousingSystem(_world);
		}

		[TearDown]
		public void TearDown()
		{
			_world.Dispose();
			_system.Dispose();
		}
		
		[Test]
		public void Test_NoHome_NoResident()
		{
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(resident.Has<Resident>());
		}

		[Test]
		public void Test_Home_Resident()
		{
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			var housing = _world.CreateEntity();
			housing.Set(new Housing(1));
			housing.Set<Position>();
			
			_system.Update(0);
			
			Assert.IsTrue(resident.Has<Resident>());
			Assert.AreEqual(resident.Get<Resident>().Home, housing);
		}
		
		[Test]
		public void Test_NoPerson_NoResident()
		{
			var house = _world.CreateEntity();
			var housing = new Housing(1);
			house.Set(housing);
			house.Set<Position>();
			
			_system.Update(0);
			
			Assert.IsTrue(housing.HasEmptyBeds);
		}
		
		[Test]
		public void Test_Person_Resident()
		{
			var house = _world.CreateEntity();
			var housing = new Housing(1);
			house.Set(housing);
			house.Set<Position>();
			
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(housing.HasEmptyBeds);
		}
		
		[Test]
		public void Test_HousingDisposed_Unemployed()
		{
			var house = _world.CreateEntity();
			var housing = new Housing(1);
			house.Set(housing);
			house.Set<Position>();
			
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			house.Dispose();
			
			_system.Update(0);
			
			Assert.IsFalse(resident.Has<Resident>());
		}
		
		[Test]
		public void Test_ResidentDisposed_EmptyHousing()
		{
			var house = _world.CreateEntity();
			var housing = new Housing(1);
			house.Set(housing);
			house.Set<Position>();
			
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(housing.HasEmptyBeds);
			resident.Dispose();
			
			_system.Update(0);
			
			Assert.IsTrue(housing.HasEmptyBeds);
		}
	}
}
