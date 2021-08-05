using CityBuilder.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems.GodotInterface
{
	public sealed partial class SpriteCreationSystem : AEntitySetSystem<float>
	{
		[ConstructorParameter]
		private readonly Node _node;

		[Update] [UseBuffer]
		private void Update(in Entity entity, in Texture texture, in Position position)
		{
			if (entity.Has<Sprite>())
			{
				entity.Get<Sprite>().QueueFree();
			}

			var sprite = new Sprite
			{
				Texture = texture,
				Position = position.Value.ToGodotVector()
			};

			if (entity.Has<Agent>())
			{
				sprite.ZIndex = 1;
			}

			_node.AddChild(sprite);

			entity.Set(sprite);
			entity.Remove<Texture>();
		}
	}
}
