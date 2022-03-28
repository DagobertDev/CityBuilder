using CityBuilder.Components;
using CityBuilder.Core.Components;
using DefaultEcs;
using DefaultEcs.System;
using Godot;

namespace CityBuilder.Systems;

public sealed partial class SizeFromSpriteSystem : AEntitySetSystem<float>
{
	[Update, UseBuffer]
	private static void Update(in Entity entity, [Added, Changed] in Sprite sprite)
	{
		var size = sprite.Texture.GetSize().ToNumericsVector();
		entity.Set<Size>(size);
	}
}
