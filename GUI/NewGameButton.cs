using CityBuilder;
using CityBuilder.Messages;
using Godot;

public class NewGameButton : Button
{
	public override void _Pressed()
	{
		Game.World.Publish(new StartNewGameMessage());
	}
}
