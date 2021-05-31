using System.IO;
using CityBuilder.Models;
using DefaultEcs;
using Godot;
using Directory = System.IO.Directory;
using World = DefaultEcs.World;

namespace CityBuilder.Systems
{
	public class ModLoader
	{
		private const string BlueprintFileExtension = "bp";

		private string ModDirectory => ProjectSettings.GlobalizePath("user://mods");
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

			return nameObject is not string name ? null : new Blueprint(name, _world.CreateEntity());
		}
	}
}
