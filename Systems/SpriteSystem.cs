using DefaultEcs;
using DefaultEcs.Command;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	[With(typeof(Texture))]
	[With(typeof(Transform2D))]
	[Without(typeof(Sprite))]
	public class SpriteSystem : AEntitySetSystem<float>
	{
		private readonly Node _node;

		public SpriteSystem(World world, Node node) : base(world)
		{
			_node = node;
		}

		private readonly EntityCommandRecorder _recorder = new();

		protected override void Update(float state, in Entity entity)
		{
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
		}

		protected override void PostUpdate(float state)
		{
			_recorder.Execute();
		}
	}
}
