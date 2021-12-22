using System;
using System.Collections.Generic;
using DefaultEcs;

namespace CityBuilder.Core.Components;

public class BehaviorQueue : Queue<Action<Entity>> { }