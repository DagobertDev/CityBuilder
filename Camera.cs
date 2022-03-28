using System;
using CityBuilder.GUI;
using Godot;

namespace CityBuilder;

public class Camera : Camera2D
{
	[Export(PropertyHint.Range, "0, 1024")]
	public int Speed { get; private set; }

	[Export(PropertyHint.Range, "0, 1024")]
	public float MinimumZoom { get; private set; }

	[Export(PropertyHint.Range, "0, 1024")]
	public float MaximumZoom { get; private set; }

	[Export(PropertyHint.Range, "0, 1")]
	public float ZoomStep { get; private set; }

	public override void _Ready()
	{
		Current = true;

		if (Speed <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(Speed));
		}

		if (MinimumZoom <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(MinimumZoom));
		}

		if (MaximumZoom <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(MaximumZoom));
		}

		if (MinimumZoom > MaximumZoom)
		{
			throw new ArgumentException($"{nameof(MinimumZoom)} can't be bigger than {nameof(MaximumZoom)}.");
		}

		if (ZoomStep is <= 0 or >= 1)
		{
			throw new ArgumentOutOfRangeException(nameof(ZoomStep));
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputAction.ZoomIn))
		{
			Zoom *= 1 - ZoomStep;
		}

		else if (@event.IsActionPressed(InputAction.ZoomOut))
		{
			Zoom /= 1 - ZoomStep;
		}

		else if (@event is InputEventMouseMotion motion && Input.IsActionPressed(InputAction.MouseclickRight))
		{
			GlobalPosition -= motion.Relative * Zoom;
		}
	}

	public override void _Process(float delta)
	{
		var direction = Input.GetVector(
			InputAction.CameraLeft,
			InputAction.CameraRight,
			InputAction.CameraUp,
			InputAction.CameraDown);

		Position += Speed * delta * direction;
	}
}
