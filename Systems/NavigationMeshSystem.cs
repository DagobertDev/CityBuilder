using System;
using CityBuilder.Components;
using CityBuilder.Core.Components;
using CityBuilder.Core.Components.AI;
using DefaultEcs;
using DefaultEcs.System;
using Godot;
using World = DefaultEcs.World;

namespace CityBuilder.Systems;

[Without(typeof(Speed))]
public sealed partial class NavigationMeshSystem : AEntitySetSystem<float>
{
	[ConstructorParameter]
	private readonly Map _map;

	public NavigationMeshSystem(World world, Map map) : base(world, CreateEntityContainer, true)
	{
		_map = map;

		world.SubscribeComponentRemoved((in Entity _, in NavigationOutlineId id) =>
		{
			_map.RemoveNavigationOutline(id);
		});
	}

	[Update, UseBuffer]
	private void Update(in Entity entity, in Position positionStruct, [Added, Changed] in Size size,
		in Rotation rotation)
	{
		var position = positionStruct.Value.ToGodotVector();
		var angle = rotation.Radians;

		var v1 = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
		var v2 = new Vector2(-v1.y, v1.x);

		v1 *= size.Value.X / 2;
		v2 *= size.Value.Y / 2;

		var outline = new[]
		{
			position + v1 + v2,
			position,
			position - v1 + v2,
			position - v1 - v2,
			position + v1 - v2,
		};

		var index = _map.AddNavigationOutline(outline);
		entity.Set<NavigationOutlineId>(index);
	}
}
