using System;
using CityBuilder.Core.Components.Inventory;
using CityBuilder.Core.Systems;
using DefaultEcs;
using NUnit.Framework;

namespace CityBuilder.Tests;

public class InventoryTests
{
	private World _world;
	private InventorySystem _system;

	[SetUp]
	public void Setup()
	{
		_world = new World();
		_system = new InventorySystem(_world);
	}

	[TearDown]
	public void TearDown()
	{
		_world.Dispose();
	}

	[Test]
	public void Test_SetName([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		var result = _system.SetGood(owner, good, amount);

		Assert.AreEqual(good, result.Get<Good>().Name);
	}

	[Test]
	public void Test_SetAmount([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		var result = _system.SetGood(owner, good, amount);

		Assert.AreEqual(amount, result.Get<Amount>().Value);
	}

	[TestCase(-1)]
	public void Test_SetNegativeAmountThrows(int amount)
	{
		var owner = _world.CreateEntity();

		Assert.Throws<ArgumentOutOfRangeException>(() => _system.SetGood(owner, "", amount));
	}

	[Test]
	public void Test_SetExistingAmount([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount,
		[Range(0, 2)] int newAmount)
	{
		var owner = _world.CreateEntity();

		_system.SetGood(owner, good, amount);
		var result = _system.SetGood(owner, good, newAmount);

		Assert.AreEqual(newAmount, result.Get<Amount>().Value);
	}

	[Test]
	public void Test_CanGet([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		_system.SetGood(owner, good, amount);
		var result = _system.GetGood(owner, good);

		Assert.IsTrue(result is { IsAlive: true });
	}

	[Test]
	public void Test_GetName([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		var result = _system.SetGood(owner, good, amount);

		Assert.AreEqual(good, result.Get<Good>().Name);
	}

	[Test]
	public void Test_GetAmount([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		_system.SetGood(owner, good, amount);
		var result = _system.GetGood(owner, good);

		Assert.AreEqual(amount, result.Get<Amount>().Value);
	}

	[Test]
	public void Test_GetOwner([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount)
	{
		var owner = _world.CreateEntity();

		_system.SetGood(owner, good, amount);
		var result = _system.GetGood(owner, good);

		Assert.AreEqual(owner, result.Get<Owner>().Value);
	}

	[Test]
	public void Test_GetFromMultiple([Values("Iron", "Wood")] string good, [Range(0, 2)] int amount,
		[Values(1, 2)] int ownerToGet, [Values(2, 3)] int totalOwners)
	{
		Entity owner = default;

		for (var i = 1; i <= totalOwners; i++)
		{
			var entity = _world.CreateEntity();
			_system.SetGood(entity, good, amount);

			if (i == ownerToGet)
			{
				owner = entity;
			}
		}

		var result = _system.GetGood(owner, good);

		Assert.AreEqual(owner, result.Get<Owner>().Value);
	}
}
