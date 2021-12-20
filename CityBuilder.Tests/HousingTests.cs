using CityBuilder.Core.Components;
using CityBuilder.Core.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class HousingTests
	{
		private World _world;
		private HousingSystem _system;

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
		public void Test_EmptyHousingAdded()
		{
			var house = _world.CreateEntity();
			house.Set<Position>();
			house.Set(new Housing(1));
			
			Assert.That(house.Get<Housing>().HasEmptyBeds);
		}
		
		[Test]
		public void Test_NoHome_NoResident()
		{
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			Assert.That(resident.Has<Resident>(), Is.False);
		}

		[Test]
		public void Test_PersonHome_Resident()
		{
			var house = _world.CreateEntity();
			house.Set<Position>();
			house.Set(new Housing(1));

			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assert.Multiple(() =>
			{
				Assert.That(_system.GetResidents(house), Is.Not.Empty);
				Assert.That(house.Get<Housing>().HasEmptyBeds, Is.False);
				Assert.That(person.Has<Resident>());
			});
		}
		
		[Test]
		public void Test_ResidentHome_IsHousing()
		{
			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			var housing = _world.CreateEntity();
			housing.Set<Position>();
			housing.Set(new Housing(1));

			_system.Update(0);
			
			Assume.That(resident.Has<Resident>());
			Assert.That(resident.Get<Resident>().Home, Is.EqualTo(housing));
		}
		
		
		[Test]
		public void Test_NoPerson_NoResident()
		{
			var house = _world.CreateEntity();
			house.Set<Position>();
			house.Set(new Housing(1));

			_system.Update(0);
			
			Assert.Multiple(() =>
			{
				Assert.That(_system.GetResidents(house), Is.Empty);
				Assert.That(house.Get<Housing>().HasEmptyBeds);
			});
		}

		[Test]
		public void Test_HousingDisposed_Homeless()
		{
			var house = _world.CreateEntity();
			house.Set<Position>();
			house.Set(new Housing(1));

			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assume.That(person.Has<Resident>());
			
			house.Dispose();
			
			_system.Update(0);
			
			Assert.That(person.Has<Resident>(), Is.False);
		}
		
		[Test]
		public void Test_ResidentDisposed_EmptyHousing()
		{
			var house = _world.CreateEntity();
			house.Set<Position>();
			house.Set(new Housing(1));

			var resident = _world.CreateEntity();
			resident.Set<Agent>();
			resident.Set<Position>();

			_system.Update(0);
			
			Assume.That(_system.GetResidents(house), Is.Not.Empty);
			Assume.That(house.Get<Housing>().HasEmptyBeds, Is.False);
			
			resident.Dispose();
			
			_system.Update(0);
			
			Assert.That(_system.GetResidents(house), Is.Empty);
			Assert.That(house.Get<Housing>().HasEmptyBeds, Is.True);
		}
		
		[Test]
		public void Test_ChangeResident()
		{
			var houseOne = _world.CreateEntity();
			houseOne.Set<Position>();
			houseOne.Set(new Housing(1));
			
			var person = _world.CreateEntity();
			person.Set<Agent>();
			person.Set<Position>();

			_system.Update(0);
			
			Assume.That(person.Has<Resident>());
			Assume.That(person.Get<Resident>().Home, Is.EqualTo(houseOne));
			Assume.That(houseOne.Get<Housing>().HasEmptyBeds, Is.False);
			
			var houseTwo = _world.CreateEntity();
			houseTwo.Set<Position>();
			houseTwo.Set(new Housing(1));
			
			person.Set(new Resident(houseTwo));
			
			Assert.Multiple(() =>
			{
				Assert.That(houseOne.Get<Housing>().HasEmptyBeds);
				Assert.That(houseTwo.Get<Housing>().HasEmptyBeds, Is.False);
			});
		}
	}
}
