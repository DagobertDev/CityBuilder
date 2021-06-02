using CityBuilder.Models;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Systems
{
	public class TirednessSystem : AComponentSystem<float, Tiredness>
	{
		public TirednessSystem(World world) : base(world) { }

		protected override void Update(float state, ref Tiredness component)
		{
			component += state;
		}
	}
}
