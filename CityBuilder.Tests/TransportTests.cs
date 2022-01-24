using System.Numerics;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests;

[Ignore("Transport got reworked")]
// TODO: Update transport tests
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
		source.Set<Position>();

		var sourceInventory = _inventorySystem.SetGood(source, good, amount);
		sourceInventory.Set<InventoryPriority>(Priority.Low);

		var market = _world.CreateEntity();
		market.Set<Position>();
		market.Set<Market>();
		var marketInventory = _inventorySystem.SetGood(market, good, 0);

		_transportSystem.Update(0);

		Assert.That(marketInventory.Get<Amount>().Value, Is.EqualTo(amount));
	}

	[Test]
	public void Test_TransportToMarketWithStartAmount([Values("Food", "Wood")] string good, [Range(1, 3)] int amount,
		[Range(1, 3)] int startAmount)
	{
		var source = _world.CreateEntity();
		source.Set<Position>();

		var sourceInventory = _inventorySystem.SetGood(source, good, amount);
		sourceInventory.Set<InventoryPriority>(Priority.Low);

		var market = _world.CreateEntity();
		market.Set<Position>();
		market.Set<Market>();
		var marketInventory = _inventorySystem.SetGood(market, good, startAmount);

		_transportSystem.Update(0);

		Assert.That(marketInventory.Get<Amount>().Value, Is.EqualTo(amount + startAmount));
	}

	[Test]
	public void Test_NoMarketDoesNotThrow([Values("Food", "Wood")] string good, [Range(1, 3)] int amount)
	{
		var source = _world.CreateEntity();
		source.Set<Position>();

		_inventorySystem.SetGood(source, good, amount);

		Assert.That(() => _transportSystem.Update(0), Throws.Nothing);
	}

	[Test]
	[Ignore("Transport got reworked")]
	public void Test_TransportChoosesClosestMarket([Values("Food", "Wood")] string good, [Range(1, 3)] int amount)
	{
		var source = _world.CreateEntity();
		source.Set(new Position(Vector2.Zero));

		var sourceInventory = _inventorySystem.SetGood(source, good, amount);
		sourceInventory.Set<InventoryPriority>(Priority.Low);

		var firstMarket = _world.CreateEntity();
		firstMarket.Set(new Position(Vector2.Zero));
		firstMarket.Set<Market>();
		var firstMarketInventory = _inventorySystem.SetGood(firstMarket, good, 0);

		var secondMarket = _world.CreateEntity();
		secondMarket.Set(new Position(Vector2.One));
		secondMarket.Set<Market>();
		var secondMarketInventory = _inventorySystem.SetGood(secondMarket, good, 0);

		_transportSystem.Update(0);

		Assert.That(firstMarketInventory.Get<Amount>().Value, Is.EqualTo(amount));
		Assert.That(secondMarketInventory.Get<Amount>().Value, Is.Zero);
	}
}
