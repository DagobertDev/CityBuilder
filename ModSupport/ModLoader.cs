using System.IO;
using CityBuilder.Components;
using CityBuilder.Components.Behaviors;
using CityBuilder.Components.Flags;
using CityBuilder.Components.Production;
using CityBuilder.Systems.UI;
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

		private string ModDirectory { get; }
		private readonly World _world = new();

		public ModLoader(TextureManager textureManager, string modDirectory)
		{
			ModDirectory = modDirectory;
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

				Game.Publisher.Publish(blueprint);
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
			if (file.HasSectionKey("blueprint", "remove"))
			{
				entity.Set<RemoveRequest>();
			}
			
			var textureObject = file.GetValue("blueprint", "texture");

			if (textureObject is string texturePath)
			{
				texturePath = path.Replace(Path.GetFileName(path), texturePath);
				entity.Set(ManagedResource<Texture>.Create(texturePath));
			}

			if (file.HasSection("construction"))
			{
				if (file.GetValue("construction", "workers") is not int workers)
				{
					workers = 1;
				}
				
				if (file.GetValue("construction", "duration") is not int duration)
				{
					duration = 1;
				}
				
				entity.Set(new Construction(workers, duration));
			}

			if (file.HasSection("agent"))
			{
				entity.Set<Idling>();
				entity.Set(new BehaviorQueue());
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

				entity.Set<Hunger>();
				entity.Set<Tiredness>();
			}

			if (file.HasSectionKey("housing", "beds"))
			{
				var bedsObject = file.GetValue("housing", "beds");

				if (bedsObject is int beds)
				{
					entity.Set(new Housing(beds));
				}
			}

			if (file.HasSection("job"))
			{
				var workersObject = file.GetValue("job", "workers");

				if (workersObject is int workers)
				{
					entity.Set(new Workplace(workers));
				}

				if (file.HasSectionKey("job", "output") 
				    && file.HasSectionKey("job", "output_amount")
				    && file.HasSectionKey("job", "difficulty"))
				{
					var goodObject = file.GetValue("job", "output");
					var outputAmountObject = file.GetValue("job", "output_amount");
					var difficultyObject = file.GetValue("job", "difficulty");

					if (goodObject is string good 
					    && outputAmountObject is int outputAmount
					    && difficultyObject is int difficulty)
					{
						entity.Set(new Output(good, outputAmount, difficulty));
					}
				}
			}

			if (file.HasSection("market"))
			{
				entity.Set<Market>();
			}
		}
	}
}
