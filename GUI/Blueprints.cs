using System.Collections.Generic;
using CityBuilder.Messages;
using CityBuilder.ModSupport;
using Godot;

namespace CityBuilder.GUI;

public class Blueprints : HBoxContainer
{
	private readonly Dictionary<int, Blueprint> _blueprintInfos = new();

	public override void _Ready()
	{
		foreach (var blueprint in Global.Blueprints.Values)
		{
			On(blueprint);
		}
	}

	private void On(in Blueprint blueprint)
	{
		var categoryNode = GetOrCreateCategory(blueprint.Category ?? "Common");

		var id = _blueprintInfos.Count;
		_blueprintInfos[id] = blueprint;

		var texture = blueprint.Texture;

		var textureCopy = (ImageTexture)texture.Duplicate();
		textureCopy.SetSizeOverride(new Vector2(32, 32));
		categoryNode.GetPopup().AddIconItem(textureCopy, blueprint.Name, id);
	}

	private MenuButton GetOrCreateCategory(string category)
	{
		if (GetNodeOrNull<MenuButton>(category) is not { } categoryNode)
		{
			categoryNode = new MenuButton
			{
				Name = category,
				Text = category,
				Flat = false,
				RectMinSize = new Vector2(100, 100),
			};
			AddChild(categoryNode);
			categoryNode.GetPopup().Connect("id_pressed", this, nameof(PublishSelection));
		}

		return categoryNode;
	}

	private void PublishSelection(int id)
	{
		var blueprint = _blueprintInfos[id];
		Game.Publisher.Publish(new BlueprintSelectedMessage(blueprint));
	}
}
