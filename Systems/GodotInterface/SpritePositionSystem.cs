using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems.GodotInterface
{
	[WhenAdded(typeof(Transform2D))]
	[WhenChanged(typeof(Transform2D))]
	[With(typeof(Sprite))]
	[WhenAdded(typeof(Sprite))]
	[WhenChanged(typeof(Sprite))]
	public class SpritePositionSystem : AEntitySetSystem<float>
	{
		public SpritePositionSystem(World world) : base(world) { }

		protected override void Update(float state, in Entity entity)
		{
			entity.Get<Sprite>().Transform = entity.Get<Transform2D>();
		}
	}
}
