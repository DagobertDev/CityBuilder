using System;
using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Components
{
	public class BehaviorQueue : Queue<Action<Entity>> { }
}
