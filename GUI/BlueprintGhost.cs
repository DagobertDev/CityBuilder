using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Core.Components;
using CityBuilder.Core.Systems;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using DefaultEcs;
using Godot;
using Vector2 = System.Numerics.Vector2;

namespace CityBuilder.GUI;

public class BlueprintGhost : Sprite
{
	private Blueprint? Blueprint { get; set; }

	private bool _canBuild;

	private bool CanBuild
	{
		get => _canBuild;
		set
		{
			if (value != _canBuild)
			{
				SelfModulate = Colors.White.LinearInterpolate(Colors.Transparent, value ? 0.1f : 0.4f);
				_canBuild = value;
			}
		}
	}

	public BlueprintGhost()
	{
		Game.Publisher.Subscribe(this);
	}

	public override void _Ready()
	{
		SetProcess(false);
		SetProcessInput(false);
	}

	[Subscribe]
	private void On(in BlueprintSelectedMessage message)
	{
		Enable(message.Blueprint);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(InputAction.MouseclickLeft))
		{
			if (CanBuild)
			{
				Build();
			}

			GetTree().SetInputAsHandled();
		}

		else if (@event.IsActionPressed(InputAction.MouseclickRight))
		{
			Disable();
			GetTree().SetInputAsHandled();
		}

		else if (@event.IsActionPressed(InputAction.RotateBuilding))
		{
			RotationDegrees = (RotationDegrees + 90) % 360;
		}
	}

	private void Build()
	{
		if (Blueprint == null)
		{
			throw new ArgumentNullException(nameof(Blueprint));
		}

		Game.Publisher.Publish(new BlueprintPlacedMessage(Blueprint,
			new Position(GlobalPosition.ToNumericsVector()),
			SteppedRotation));
	}

	private void Enable(Blueprint blueprint)
	{
		Blueprint = blueprint;
		Texture = blueprint.Texture;
		Visible = true;
		SetProcess(true);
		SetProcessInput(true);
	}

	private void Disable()
	{
		Blueprint = null;
		Visible = false;
		SetProcess(false);
		SetProcessInput(false);
		RotationDegrees = 0;
	}

	public override void _Process(float delta)
	{
		var position = GetGlobalMousePosition();
		GlobalPosition = position;

		var size = Texture.GetSize().ToNumericsVector();

		if (SteppedRotation.Value is 90 or 270)
		{
			size = new Vector2(size.Y, size.X);
		}

		CanBuild = Blueprint is { RemoveRequest: { } } or { CollectResource: { } } || Game.World
			.Get<ICollisionSystem>()
			.GetEntities(new HitBox(position.ToNumericsVector(), size, default)).All(entity => entity.Has<Agent>());
	}

	private Rotation SteppedRotation =>
		RotationDegrees switch
		{
			> 89 and < 91 => 90,
			> 179 and < 181 => 180,
			> 269 and < 271 => 270,
			_ => 0,
		};
}
