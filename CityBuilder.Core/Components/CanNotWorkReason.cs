using System;

namespace CityBuilder.Core.Components;

[Flags]
public enum CanNotWorkReason
{
	None = 0,
	NoInput = 1 << 0,
	InventoryFull = 1 << 1,
}
