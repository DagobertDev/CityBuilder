using System;
using System.Collections.Generic;
using System.Linq;
using CityBuilder.Components.Inventory;
using CityBuilder.Messages;
using CityBuilder.Systems;
using DefaultEcs;
using Godot;

namespace CityBuilder.GUI
{
	public class DebugConsole : LineEdit
	{
		private readonly IList<DebugCommand> _commands;
		private Entity? _selectedEntity;

		public DebugConsole()
		{
			Game.World.Subscribe(this);

			_commands = new List<DebugCommand>();
			_commands.Add(new DebugCommand("print", GD.Print));
			_commands.Add(new DebugCommand("pause", () =>
			{
				var paused = GetTree().Paused;
				GetTree().Paused = !paused;
				GD.Print(paused ? "Game continued" : "Game paused");
			}));
			_commands.Add(new DebugCommand("set_inventory", args =>
			{
				if (!_selectedEntity.HasValue || args.Length < 2)
				{
					return;
				}

				if (!int.TryParse(args[1], out var amount))
				{
					return;
				}
				
				var good = args[0];
				Game.World.Get<IInventorySystem>().SetGood(_selectedEntity.Value, good, amount);
			}));
			
			_commands.Add(new DebugCommand("view_inventory", () =>
			{
				if (!_selectedEntity.HasValue)
				{
					return;
				}

				var inventory = Game.World.Get<IInventorySystem>().GetGoods(_selectedEntity.Value);
				
				GD.Print(string.Join("\n", inventory.Select(entity => $"{entity.Get<Good>().Name}: {entity.Get<Amount>().Value}")));
			}));

			Connect("text_entered", this, nameof(HandleInput));
		}

		public override void _UnhandledInput(InputEvent @event)
		{
			if (@event.IsActionPressed(InputAction.ToggleDebug))
			{
				Visible = !Visible;
			}
		}

		public void HandleInput(string text)
		{
			var command = text.Split(" ");
			var args = command.Skip(1).ToArray();

			_commands.FirstOrDefault(c => c.Id == command[0])?.Invoke(args);
		}

		[Subscribe]
		private void On(in EntitySelected selected)
		{
			_selectedEntity = selected.Entity;
		}
		
		private class DebugCommand
		{
			public string Id { get; }
			private readonly Action<string[]> _action;
			
			public DebugCommand(string id, Action action)
			{
				Id = id;
				_action = _ => action();
			}

			public DebugCommand(string id, Action<string[]> action)
			{
				Id = id;
				_action = action;
			}

			public void Invoke(params string[] args) => _action.Invoke(args);
		}
	}
}
