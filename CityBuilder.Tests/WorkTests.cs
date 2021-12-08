using CityBuilder.Components;
using CityBuilder.Systems;
using DefaultEcs;
using DefaultEcs.System;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class WorkTests
	{
		private World _world;
		private ISystem<float> _system;

		[SetUp]
		public void Setup()
		{
			_world = new World();
			_system = new WorkSystem(_world);
		}

		[TearDown]
		public void TearDown()
		{
			_world.Dispose();
			_system.Dispose();
		}
		
		[Test]
		public void Test_NoWork_NoEmployee()
		{
			var worker = _world.CreateEntity();
			worker.Set<Agent>();
			worker.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(worker.Has<Employee>());
		}

		[Test]
		public void Test_Work_Employee()
		{
			var worker = _world.CreateEntity();
			worker.Set<Agent>();
			worker.Set<Position>();

			var workplace = _world.CreateEntity();
			workplace.Set(new Workplace(1));
			workplace.Set<Position>();
			
			_system.Update(0);
			
			Assert.IsTrue(worker.Has<Employee>());
			Assert.AreEqual(worker.Get<Employee>().Workplace, workplace);
		}
		
		[Test]
		public void Test_NoPerson_NoWorker()
		{
			var work = _world.CreateEntity();
			var workplace = new Workplace(1);
			work.Set(workplace);
			work.Set<Position>();
			
			_system.Update(0);
			
			Assert.IsTrue(workplace.HasEmptyWorkspace);
		}
		
		[Test]
		public void Test_Person_Employee()
		{
			var work = _world.CreateEntity();
			var workplace = new Workplace(1);
			work.Set(workplace);
			work.Set<Position>();
			
			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(workplace.HasEmptyWorkspace);
		}
		
		[Test]
		public void Test_HousingDisposed_Unemployed()
		{
			var work = _world.CreateEntity();
			var workplace = new Workplace(1);
			work.Set(workplace);
			work.Set<Position>();
			
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			work.Dispose();
			
			_system.Update(0);
			
			Assert.IsFalse(resident.Has<Employee>());
		}
		
		[Test]
		public void Test_ResidentDisposed_EmptyHousing()
		{
			var work = _world.CreateEntity();
			var workplace = new Workplace(1);
			work.Set(workplace);
			work.Set<Position>();
			
			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assert.IsFalse(workplace.HasEmptyWorkspace);
			person.Dispose();
			
			_system.Update(0);
			
			Assert.IsTrue(workplace.HasEmptyWorkspace);
		}
	}
}