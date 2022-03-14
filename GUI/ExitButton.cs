using Godot;

public class ExitButton : Button
{
	public override void _Pressed()
	{
		GetTree().Quit();
	}
}
