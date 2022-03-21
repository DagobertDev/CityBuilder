namespace CityBuilder.Core.Components;

public enum InventoryType
{
	/// <summary>
	///     No automatic transport from or to this inventory.
	/// </summary>
	Manual,

	/// <summary>
	///     Automatic transport from this inventory.
	/// </summary>
	Supply,

	/// <summary>
	///     Automatic transport from and to this inventory.
	/// </summary>
	Market,

	/// <summary>
	///     Automatic transport to this inventory
	/// </summary>
	Demand,
}
