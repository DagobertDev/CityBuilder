using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CityBuilder.GUI
{
	public class DebugConsole : LineEdit
	{
		private readonly IList<DebugCommand> _commands;

		public DebugConsole()
		{
			_commands = new List<DebugCommand>();
			_commands.Add(new DebugCommand("print", GD.Print));
			_commands.Add(new DebugCommand("pause", () => GetTree().Paused = !GetTree().Paused));

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
