using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Core.Components.Inventory;

public sealed class Inventory : SortedList<string, Entity>
{
	public Inventory() { }
	public Inventory(int capacity) : base(capacity) { }
}
