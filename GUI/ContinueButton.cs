using CityBuilder;
using CityBuilder.Messages;
using Godot;

public class ContinueButton : Button
{
	public override void _Pressed()
	{
		Game.World.Publish(new CloseInGameMenuMessage());
	}
}
