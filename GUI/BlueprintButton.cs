using System;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using Godot;

namespace CityBuilder.GUI
{
	public class BlueprintButton : Button
	{
		private Blueprint _blueprint;
		public Blueprint Blueprint
		{
			get => _blueprint;
			set
			{
				_blueprint = value;
				Text = _blueprint!.Name;
			}
		}

		public override void _Ready()
		{
			if (Blueprint == null)
			{
				throw new ArgumentNullException(nameof(Blueprint));
			}
		}

		public override void _Pressed()
		{
			Game.Publisher.Publish(new BlueprintSelectedMessage(Blueprint));
		}
	}
}
