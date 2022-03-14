using CityBuilder.GUI;
using Godot;

public class InGameMenu : Control
{
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(InputAction.Escape))
		{
			Visible = !Visible;
		}
	}
}
