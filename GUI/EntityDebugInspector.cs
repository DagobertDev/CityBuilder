using System;
using System.Collections.Generic;
using CityBuilder.Messages;
using DefaultEcs;
using DefaultEcs.Serialization;
using Godot;
using Newtonsoft.Json;
using Object = Godot.Object;

namespace CityBuilder.GUI;

public class EntityDebugInspector : WindowDialog
{
	[Export]
	public NodePath JsonTreePath { get; set; }

	private Entity? _entity;
	private readonly Timer _timer = new();
	private JsonTree JsonTree => GetNode<JsonTree>(JsonTreePath);

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

		var reader = new Reader();
		_entity.Value.ReadAllComponents(reader);
		var text = JsonConvert.SerializeObject(reader.Components, new EntityConverter());
		var godot = JSON.Parse(text);
		JsonTree.SetData(godot.Result);
	}

	[Subscribe]
	private void On(in EntitySelected entitySelected)
	{
		if (!Input.IsActionPressed(InputAction.Control))
		{
			return;
		}

		_entity = entitySelected.Entity;
		Popup_(new Rect2(RectPosition, RectSize));
		UpdateEntity();
		_timer.Start(0.3f);
	}

	private class Reader : IComponentReader
	{
		public readonly IDictionary<string, object> Components = new Dictionary<string, object>();

		public void OnRead<T>(in T component, in Entity componentOwner)
		{
			if (Components.Count == 0)
			{
				Components.Add("Entity", componentOwner);
			}

			if (component is Object)
			{
				return;
			}

			if (component is not null)
			{
				Components.Add(typeof(T).Name, component);
			}
		}
	}

	private class EntityConverter : JsonConverter<Entity>
	{
		public override void WriteJson(JsonWriter writer, Entity entity, JsonSerializer serializer)
		{
			serializer.Serialize(writer, entity.ToString());
		}

		public override Entity ReadJson(JsonReader reader, Type objectType, Entity existingValue,
			bool hasExistingValue,
			JsonSerializer serializer) =>
			throw new NotSupportedException();
	}
}
