using System.Collections.Generic;

namespace CityBuilder.Core.Components.Production;

public readonly record struct Input(Dictionary<string, int> Value);
