using System;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Components.Flags;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using CityBuilder.Systems;
using DefaultEcs;
using Godot;

namespace CityBuilder.GUI
{
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
		}

		private void Build()
		{
			if (Blueprint == null)
			{
				throw new ArgumentNullException(nameof(Blueprint));
			}

			Game.Publisher.Publish(new BlueprintPlacedMessage(Blueprint, GlobalTransform));
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
			var position = GetGlobalMousePosition();
			GlobalPosition = position;

			CanBuild = Blueprint!.Entity.Has<RemoveRequest>() || Game.World.Get<CollisionSystem>()
				.GetEntities(new HitBox(position, Texture.GetSize(), default)).All(entity => entity.Has<Agent>());
		}
	}
}
