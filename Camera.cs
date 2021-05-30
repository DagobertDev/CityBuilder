using System;
using Godot;

namespace CityBuilder
{
	public class Camera : Camera2D
	{
		public const string CameraLeft = "camera_left";
		public const string CameraRight = "camera_right";
		public const string CameraUp = "camera_up";
		public const string CameraDown = "camera_down";
		public const string ZoomIn = "zoom_in";
		public const string ZoomOut = "zoom_out";

		private Vector2 _direction;

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

			if (ZoomStep <= 0 || ZoomStep >= 1)
			{
				throw new ArgumentOutOfRangeException(nameof(ZoomStep));
			}
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed(ZoomIn))
			{
				Zoom *= 1 - ZoomStep;
			}

			else if (@event.IsActionPressed(ZoomOut))
			{
				Zoom /= 1 - ZoomStep;
			}


			else if (@event.IsActionPressed(CameraUp))
			{
				_direction.y = -1;
			}

			else if (@event.IsActionReleased(CameraUp))
			{
				_direction.y = 0;
			}

			else if (@event.IsActionPressed(CameraDown))
			{
				_direction.y = 1;
			}

			else if (@event.IsActionReleased(CameraDown))
			{
				_direction.y = 0;
			}

			else if (@event.IsActionPressed(CameraLeft))
			{
				_direction.x = -1;
			}

			else if (@event.IsActionReleased(CameraLeft))
			{
				_direction.x = 0;
			}

			else if (@event.IsActionPressed(CameraRight))
			{
				_direction.x = 1;
			}

			else if (@event.IsActionReleased(CameraRight))
			{
				_direction.x = 0;
			}
		}

		public override void _Process(float delta)
		{
			Position += Speed * _direction;
		}
	}
}
