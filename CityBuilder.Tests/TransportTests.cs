using CityBuilder.Components;
using CityBuilder.Components.Inventory;
using CityBuilder.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests
{
	public class TransportTests
	{
		private World _world;
		private TransportSystem _transportSystem;
		private IInventorySystem _inventorySystem;

		[SetUp]
		public void Setup()
		{
			_world = new World();
			_transportSystem = new TransportSystem(_world);
			_inventorySystem = new InventorySystem(_world);
		}

		[TearDown]
		public void TearDown()
		{
			_world.Dispose();
			_transportSystem.Dispose();
		}

		[Test]
		public void Test_TransportToMarket([Values("Food", "Wood")] string good, [Range(1, 3)] int amount)
		{
			var source = _world.CreateEntity();

			_inventorySystem.SetGood(source, good, amount);

			var market = _world.CreateEntity();
			market.Set<Market>();
			var marketInventory = _inventorySystem.SetGood(market, good, 0);
			
			_transportSystem.Update(0);

			Assert.That(marketInventory.Get<Amount>().Value, Is.EqualTo(amount));
		}
		
		[Test]
		public void Test_TransportToMarketWithStartAmount([Values("Food", "Wood")] string good, [Range(1, 3)] int amount, [Range(1, 3)] int startAmount)
		{
			var source = _world.CreateEntity();

			_inventorySystem.SetGood(source, good, amount);

			var market = _world.CreateEntity();
			market.Set<Market>();
			var marketInventory = _inventorySystem.SetGood(market, good, startAmount);
			
			_transportSystem.Update(0);

			Assert.That(marketInventory.Get<Amount>().Value, Is.EqualTo(amount + startAmount));
		}
		
		[Test]
		public void Test_NoMarketDoesNotThrow([Values("Food", "Wood")] string good, [Range(1, 3)] int amount)
		{
			var source = _world.CreateEntity();

			_inventorySystem.SetGood(source, good, amount);
			
			Assert.That(() => _transportSystem.Update(0), Throws.Nothing);
		}
	}
}
