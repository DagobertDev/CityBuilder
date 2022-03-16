using System.Linq;
using Godot;

public class Map : YSort
{
	private Navigation2D? _navigation;
	private Navigation2D Navigation => _navigation ??= GetNode<Navigation2D>("Navigation");
	public Node EntityRoot => Navigation;

	public void Initialize(Vector2 size)
	{
		var navigationPolygon = new NavigationPolygon();
		navigationPolygon.AddOutline(new[]
		{
			Vector2.Zero,
			Vector2.Right * size.x,
			size,
			Vector2.Down * size.y,
		});

		navigationPolygon.MakePolygonsFromOutlines();

		Navigation.AddChild(new NavigationPolygonInstance
		{
			Navpoly = navigationPolygon,
		});
	}

	public void Reset()
	{
		foreach (var child in Navigation.GetChildren().Cast<Node>())
		{
			Navigation.RemoveChild(child);
		}
	}
}
