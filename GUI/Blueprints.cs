using System;
using CityBuilder.Models;
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

			CityBuilder.Publisher.Subscribe<Blueprint>(On);
		}

		private void On(in Blueprint blueprint)
		{
			var button = BlueprintButton!.Instance<BlueprintButton>();
			button.Blueprint = blueprint;
			AddChild(button);
		}
	}
}
