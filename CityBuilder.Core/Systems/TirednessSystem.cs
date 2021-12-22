using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems;

public class TirednessSystem : AComponentSystem<float, Tiredness>
{
	private readonly float _rate;
		
	public TirednessSystem(World world, float rate) : base(world)
	{
		_rate = rate;
	}

	protected override void Update(float state, ref Tiredness component)
	{
		component += state * _rate;
	}
}