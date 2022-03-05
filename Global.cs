using System.Collections.Generic;
using CityBuilder.Components;

namespace CityBuilder
{
	public static class Global
	{
		public static readonly IDictionary<string, GoodDescription> GoodDescriptions =
			new Dictionary<string, GoodDescription>();
	}
}
