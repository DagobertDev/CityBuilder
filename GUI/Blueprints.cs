using System;
using CityBuilder.ModSupport;
using Godot;

namespace CityBuilder.GUI
{
	public class Blueprints : HBoxContainer
	{
		[Export]
		private PackedScene? BlueprintButton { get; set; }

		public override void _EnterTree()
		{
			if (BlueprintButton == null)
			{
				throw new ArgumentNullException(nameof(BlueprintButton));
			}

			Game.Publisher.Subscribe<Blueprint>(On);
		}

		private void On(in Blueprint blueprint)
		{
			var button = BlueprintButton!.Instance<BlueprintButton>();
			button.Blueprint = blueprint;
			AddChild(button);
		}
	}
}
