using System.Collections.Generic;
using System.Linq;
using Godot;

public class Map : YSort
{
	private Navigation2D? _navigation;
	private Navigation2D Navigation => _navigation ??= GetNode<Navigation2D>("Navigation");
	public Node EntityRoot => Navigation;

	private NavigationPolygonInstance _navigationPolygonInstance;

	public Map()
	{
		_navigationPolygonInstance = new NavigationPolygonInstance
		{
			Navpoly = new NavigationPolygon(),
		};
	}

	public void Initialize(Vector2 size)
	{
		AddNavigationOutline(new[]
		{
			Vector2.Zero,
			Vector2.Right * size.x,
			size,
			Vector2.Down * size.y,
		});
	}

	public void Reset()
	{
		foreach (var child in Navigation.GetChildren().Cast<Node>())
		{
			Navigation.RemoveChild(child);
		}
	}

	public int AddNavigationOutline(Vector2[] outline)
	{
		var navigationPolygon = _navigationPolygonInstance.Navpoly;
		navigationPolygon.AddOutline(outline);
		navigationPolygon.MakePolygonsFromOutlines();

		_navigationPolygonInstance.QueueFree();
		_navigationPolygonInstance = new NavigationPolygonInstance
		{
			Navpoly = navigationPolygon,
		};

		Navigation.AddChild(_navigationPolygonInstance);

		_currentOutlineId++;
		_outlineIdIndex[_currentOutlineId] = navigationPolygon.GetOutlineCount() - 1;
		return _currentOutlineId;
	}

	public void RemoveNavigationOutline(int id)
	{
		var navigationPolygon = _navigationPolygonInstance.Navpoly;

		var index = _outlineIdIndex[id];

		navigationPolygon.RemoveOutline(index);

		foreach (var changed in _outlineIdIndex.Where(kv => kv.Value > index).ToList())
		{
			_outlineIdIndex[changed.Key] = changed.Value - 1;
		}

		navigationPolygon.MakePolygonsFromOutlines();

		_navigationPolygonInstance.QueueFree();
		_navigationPolygonInstance = new NavigationPolygonInstance
		{
			Navpoly = navigationPolygon,
		};

		Navigation.AddChild(_navigationPolygonInstance);
	}

	private int _currentOutlineId;
	private readonly IDictionary<int, int> _outlineIdIndex = new Dictionary<int, int>();
}
