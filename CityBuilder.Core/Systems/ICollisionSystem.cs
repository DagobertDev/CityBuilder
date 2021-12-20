using System.Collections.Generic;
using System.Numerics;
using CityBuilder.Core.Components;
using DefaultEcs;

namespace CityBuilder.Core.Systems
{
	public interface ICollisionSystem
	{
		public IEnumerable<Entity> GetEntities(HitBox hitBox);

		public IEnumerable<Entity> GetEntities(Vector2 position);
	}
}
