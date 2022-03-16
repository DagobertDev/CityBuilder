using System;
using CityBuilder;
using CityBuilder.Messages;
using Godot;

public class NewGameMenu : VBoxContainer
{
	public void StartNewGame()
	{
		var node = FindNode("Humans");

		if (node is not Range populationSetting)
		{
			throw new ApplicationException("Could not find node");
		}

		if ((int)populationSetting.Value <= 0)
		{
			GD.PushError("Population value is < 0");
			return;
		}

		Game.World.Publish(new StartNewGameMessage((int)populationSetting.Value));
	}
}
