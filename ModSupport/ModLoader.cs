using System.IO;
using CityBuilder.Components;
using CityBuilder.Components.Flags;
using CityBuilder.Systems.GodotInterface;
using DefaultEcs;
using DefaultEcs.Resource;
using Godot;
using Directory = System.IO.Directory;
using Path = System.IO.Path;
using World = DefaultEcs.World;

namespace CityBuilder.ModSupport
{
	public class ModLoader
	{
		private const string BlueprintFileExtension = "bp";

		private static string ModDirectory => ProjectSettings.GlobalizePath("res://mods");
		private readonly World _world = new();

		public ModLoader(TextureManager textureManager)
		{
			textureManager.Manage(_world);
		}

		public void LoadMods()
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
			var file = new ConfigFile();

			if (file.Load(path) != Error.Ok)
			{
				return null;
			}

			var nameObject = file.GetValue("blueprint", "name");

			if (nameObject is not string name)
			{
				return null;
			}

			var entity = _world.CreateEntity();

			PopulateEntity(entity, file, path);

			return new Blueprint(name, entity, newEntity => PopulateEntity(newEntity, file, path));
		}

		private static void PopulateEntity(Entity entity, ConfigFile file, string path)
		{
			var textureObject = file.GetValue("blueprint", "texture");

			if (textureObject is string texturePath)
			{
				texturePath = path.Replace(Path.GetFileName(path), texturePath);
				entity.Set(ManagedResource<Texture>.Create(texturePath));
			}

			if (file.HasSectionKey("agent", "speed"))
			{
				var speedObject = file.GetValue("agent", "speed");

				switch (speedObject)
				{
					case float speed:
						entity.Set(new Agent(speed));
						break;
					case int speed:
						entity.Set(new Agent(speed));
						break;
				}

				entity.Set<Tiredness>();
			}

			if (file.HasSectionKey("housing", "beds"))
			{
				var bedsObject = file.GetValue("housing", "beds");

				if (bedsObject is int beds)
				{
					entity.Set(new Housing(beds));
					entity.Set<EmptyHousing>();
				}
			}

			if (file.HasSection("job"))
			{
				var workersObject = file.GetValue("job", "workers");

				if (workersObject is int workers)
				{
					entity.Set(new Workplace(workers));
					entity.Set<EmptyWorkspace>();
				}
			}
		}
	}
}
