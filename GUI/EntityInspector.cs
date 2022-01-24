using System;
using CityBuilder;
using CityBuilder.Core.Messages;
using CityBuilder.GUI;
using DefaultEcs;
using Godot;

public class EntityInspector : WindowDialog
{
	public event EventHandler<Entity>? EntityUpdated;

	private Entity? _entity;
	private readonly Timer _timer = new();

	public override void _Ready()
	{
		Game.Publisher.Subscribe(this);

		_timer.Connect("timeout", this, nameof(UpdateEntity));
		AddChild(_timer);

		Connect("popup_hide", this, nameof(StopTimer));
	}

	private void StopTimer()
	{
		_timer.Stop();
	}

	private void UpdateEntity()
	{
		if (_entity is not { IsAlive: true })
		{
			Hide();
			return;
		}

		EntityUpdated?.Invoke(this, _entity.Value);
	}

	[Subscribe]
	private void On(in EntitySelected entitySelected)
	{
		if (Input.IsActionPressed(InputAction.Control))
		{
			return;
		}

		_entity = entitySelected.Entity;
		WindowTitle = _entity.Value.ToString();
		Popup_(new Rect2(RectPosition, RectSize));
		UpdateEntity();
		_timer.Start(0.3f);
	}
}
