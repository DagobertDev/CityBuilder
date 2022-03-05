using System.Collections.Generic;
using CityBuilder.Core.Messages;
using CityBuilder.Core.ModSupport;
using Godot;

namespace CityBuilder.GUI
{
	public class Blueprints : HBoxContainer
	{
		private readonly Dictionary<int, Blueprint> _blueprintInfos = new();

		public override void _EnterTree()
		{
			Game.Publisher.Subscribe<Blueprint>(On);
		}

		private void On(in Blueprint blueprint)
		{
			var category = blueprint.Entity.Get<BlueprintInfo>().Category;
			var categoryNode = GetOrCreateCategory(category ?? "Common");

			var id = _blueprintInfos.Count;
			_blueprintInfos[id] = blueprint;

			var texture = blueprint.Entity.Get<Texture>();

			if (texture is ImageTexture imageTexture)
			{
				var textureCopy = (ImageTexture)imageTexture.Duplicate();
				textureCopy.SetSizeOverride(new Vector2(32, 32));

				categoryNode.GetPopup().AddIconItem(textureCopy, blueprint.Name, id);
			}
			else
			{
				categoryNode.GetPopup().AddItem(blueprint.Name, id);
			}
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
}
