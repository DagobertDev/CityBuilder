using System;
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

		protected override void Update(float state, ReadOnlySpan<Entity> entities)
		{
			var recorder = new EntityCommandRecorder();

			foreach (var entity in entities)
			{
				var sprite = new Sprite
				{
					Texture = entity.Get<Texture>()
				};

				_node.AddChild(sprite);

				var record = recorder.Record(entity);
				record.Set(sprite);
			}

			recorder.Execute();
		}
	}
}
