using CityBuilder.Core.Components;
using CityBuilder.Core.Systems;
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
			work.Set<Position>();
			work.Set(new Workplace(1));

			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assert.That(work.Get<Workplace>().HasEmptyWorkspace, Is.False);
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
			work.Set<Position>();
			work.Set(new Workplace(1));

			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assert.That(work.Get<Workplace>().HasEmptyWorkspace, Is.False);
			person.Dispose();
			
			_system.Update(0);
			
			Assert.That(work.Get<Workplace>().HasEmptyWorkspace);
		}
		
		[Test]
		public void Test_ChangeEmployee()
		{
			var workOne = _world.CreateEntity();
			workOne.Set<Position>();
			workOne.Set(new Workplace(1));

			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assume.That(workOne.Get<Workplace>().HasEmptyWorkspace, Is.False);
			
			var workTwo = _world.CreateEntity();
			workTwo.Set<Position>();
			workTwo.Set(new Workplace(1));
			
			person.Set(new Employee(workTwo));

			Assert.Multiple(() =>
			{
				Assert.That(workOne.Get<Workplace>().HasEmptyWorkspace);
				Assert.That(workTwo.Get<Workplace>().HasEmptyWorkspace, Is.False);
			});
		}
	}
}
