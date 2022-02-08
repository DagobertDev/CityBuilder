using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Components.Production;
using CityBuilder.Core.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests;

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
		_system = new ProductionSystem(_world, _inventorySystem);
	}

	[TearDown]
	public void TearDown()
	{
		_world.Dispose();
		_system.Dispose();
	}

	[Test]
	public void Test_Production([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount,
		[Range(1, 3)] int difficulty)
	{
		var workplace = _world.CreateEntity();
		_inventorySystem.EnsureCreated(workplace, good);
		var output = new Output(good, amount, difficulty);
		workplace.Set(output);

		workplace.Set<WorkProgress>(1);
		_system.Update(0);

		var inventory = _inventorySystem.GetGood(workplace, good);

		Assert.That(inventory.Get<Amount>().Value, Is.EqualTo(amount));
	}
}
