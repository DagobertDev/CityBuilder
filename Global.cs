using System.Collections.Generic;
using CityBuilder.Components;
using CityBuilder.ModSupport;

namespace CityBuilder
{
	public static class Global
	{
		public static readonly IDictionary<string, GoodDescription> GoodDescriptions =
			new Dictionary<string, GoodDescription>();

		public static readonly IDictionary<string, Blueprint> Blueprints = new Dictionary<string, Blueprint>();
	}
}
