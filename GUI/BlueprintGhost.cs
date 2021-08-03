using System;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using DefaultEcs;
using Godot;

namespace CityBuilder.GUI
{
	public class BlueprintGhost : Sprite
	{
		private const string MouseclickLeft = "mouseclick_left";
		private const string MouseclickRight = "mouseclick_right";

		private Blueprint? Blueprint { get; set; }

		public BlueprintGhost()
		{
			CityBuilder.Publisher.Subscribe(this);
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
			if (@event.IsActionPressed(MouseclickLeft))
			{
				Build();
				GetTree().SetInputAsHandled();
			}

			else if (@event.IsActionPressed(MouseclickRight))
			{
				Disable();
				GetTree().SetInputAsHandled();
			}
		}

		private void Build()
		{
			if (Blueprint == null)
			{
				throw new ArgumentNullException(nameof(Blueprint));
			}

			CityBuilder.Publisher.Publish(new BlueprintPlacedMessage(Blueprint, GlobalTransform));
		}

		private void Enable(Blueprint blueprint)
		{
			Blueprint = blueprint;
			Texture = blueprint.Entity.Has<Texture>() ? blueprint.Entity.Get<Texture>() : null;
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
		}

		public override void _Process(float delta)
		{
			GlobalPosition = GetGlobalMousePosition();
		}
	}
}
