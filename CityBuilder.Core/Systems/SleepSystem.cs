using CityBuilder.Core.Components;
using CityBuilder.Core.Components.Behaviors;
using CityBuilder.Core.Components.Flags;
using DefaultEcs;
using DefaultEcs.System;

namespace CityBuilder.Core.Systems
{
	[With(typeof(Sleeping))]
	[With(typeof(IsAtHome))]
	public sealed partial class SleepSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter] 
		private readonly float _rate;
		
		[Update] [UseBuffer]
		private void Update(float state, in Entity entity, ref Tiredness tiredness)
		{
			tiredness -= state * _rate;

			if (tiredness <= 0)
			{
				entity.Remove<Sleeping>();
				entity.Set<Idling>();
			}
		}
	}
}
