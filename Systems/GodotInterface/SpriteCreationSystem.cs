using System;
using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.Command;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems.GodotInterface
{
	[With(typeof(Texture))]
	[With(typeof(Transform2D))]
	[Without(typeof(Sprite))]
	public class SpriteCreationSystem : AEntitySetSystem<float>
	{
		private readonly Node _node = CityBuilder.Instance.Map;

		public SpriteCreationSystem(World world) : base(world) { }

		private readonly EntityCommandRecorder _recorder = new();

		protected override void Update(float state, in Entity entity)
		{
			if (entity.Has<Sprite>())
			{
				throw new ApplicationException("Entity should not have sprite");
			}

			var sprite = new Sprite
			{
				Texture = entity.Get<Texture>()
			};

			if (entity.Has<Agent>())
			{
				sprite.ZIndex = 1;
			}

			_node.AddChild(sprite);

			var record = _recorder.Record(entity);
			record.Set(sprite);
			record.Remove<Texture>();
		}

		protected override void PostUpdate(float state)
		{
			_recorder.Execute();
		}
	}
}
