using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.Command;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.GodotInterface
{
	[With(typeof(Transform2D))]
	public sealed partial class SpriteCreationSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter]
		private readonly Node _node;

		private readonly EntityCommandRecorder _recorder = new();

		[Update]
		private void Update(in Entity entity, in Texture texture)
		{
			if (entity.Has<Sprite>())
			{
				entity.Get<Sprite>().QueueFree();
			}

			var sprite = new Sprite
			{
				Texture = texture
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
