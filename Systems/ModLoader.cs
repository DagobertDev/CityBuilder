using System.IO;
using CityBuilder.Models;
using DefaultEcs;
using Godot;
using Directory = System.IO.Directory;
using Path = System.IO.Path;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public class ModLoader
	{
		private const string BlueprintFileExtension = "bp";

		private string ModDirectory => ProjectSettings.GlobalizePath("res://mods");
		private readonly World _world = new();

		[Subscribe]
		private void LoadMods(in LoadMods _)
		{
			LoadMods(ModDirectory);
		}

		private void LoadMods(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			foreach (var file in Directory.EnumerateFiles(path,
				$"*.{BlueprintFileExtension}",
				SearchOption.AllDirectories))
			{
				var blueprint = ParseBlueprint(file);

				if (blueprint == null)
				{
					continue;
				}

				CityBuilder.Publisher.Publish(blueprint);
			}
		}

		private Blueprint? ParseBlueprint(string path)
		{
			using var file = new ConfigFile();

			if (file.Load(path) != Error.Ok)
			{
				return null;
			}

			var nameObject = file.GetValue("blueprint", "name");

			if (nameObject == null)
			{
				return null;
			}

			var entity = _world.CreateEntity();

			var textureObject = file.GetValue("blueprint", "texture");

			if (textureObject is string texturePath)
			{
				texturePath = path.Replace(Path.GetFileName(path), texturePath);

				var image = new Image();

				if (image.Load(texturePath) == Error.Ok)
				{
					var texture = new ImageTexture();
					texture.CreateFromImage(image);
					entity.Set<Texture>(texture);
				}
			}

			return new Blueprint((string)nameObject, entity);
		}
	}
}
