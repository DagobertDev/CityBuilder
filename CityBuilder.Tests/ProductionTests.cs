using CityBuilder.Components;
using CityBuilder.Components.Inventory;
using CityBuilder.Components.Production;
using CityBuilder.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class ProductionTests
	{
		private World _world;
		private ProductionSystem _system;
		private IInventorySystem _inventorySystem;

		[SetUp]
		public void Setup()
		{
			_world = new World();
			_inventorySystem = new InventorySystem(_world);
			_world.Set(_inventorySystem);
			_system = new ProductionSystem(_world);
		}

		[TearDown]
		public void TearDown()
		{
			_world.Dispose();
			_system.Dispose();
		}
		
		[Test]
		public void Test_InventoryInitialized([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount, [Range(1, 3)] int difficulty)
		{
			var workplace = _world.CreateEntity();
			var output = new Output(good, amount, difficulty);
			workplace.Set(output);

			var inventory = _inventorySystem.GetGood(workplace, good);

			Assert.That(inventory.HasValue);
			Assert.That(inventory.Value.Get<Amount>().Value, Is.Zero);
		}
		
		[Test]
		public void Test_Production([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount, [Range(1, 3)] int difficulty)
		{
			var workplace = _world.CreateEntity();
			var output = new Output(good, amount, difficulty);
			workplace.Set(output);

			workplace.Set(new WorkProgress(difficulty));
			_system.Update(0);

			var inventory = _inventorySystem.GetGood(workplace, good); 
			
			Assume.That(inventory.HasValue);
			Assert.That(inventory.Value.Get<Amount>().Value, Is.EqualTo(amount));
		}
	}
}
