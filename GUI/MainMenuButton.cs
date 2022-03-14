using CityBuilder;
using CityBuilder.Messages;
using Godot;

public class MainMenuButton : Button
{
	public override void _Pressed()
	{
		Game.World.Publish(new OpenMainMenuMessage());
	}
}
