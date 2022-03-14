using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Systems.UI;
using Godot;
using Newtonsoft.Json;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Path = System.IO.Path;
using World = DefaultEcs.World;

namespace CityBuilder.ModSupport
{
	public class ModLoader
	{
		private const string BuildingsFile = "buildings.json";
		private const string GoodsFile = "goods.json";

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

			foreach (var file in Directory.EnumerateFiles(path, GoodsFile,
						 SearchOption.AllDirectories))
			{
				var textureConverter = new StringToTextureConverter(Path.GetDirectoryName(file) ?? string.Empty);
				var data = File.ReadAllText(file);

				var goods = JsonConvert.DeserializeObject<IEnumerable<GoodDescription>>(data, textureConverter);

				if (goods is null)
				{
					continue;
				}

				foreach (var good in goods)
				{
					Global.GoodDescriptions[good.Id] = good;
				}
			}

			foreach (var blueprint in Directory.EnumerateFiles(path, BuildingsFile, SearchOption.AllDirectories)
						 .SelectMany(ParseBlueprints))
			{
				Game.Publisher.Publish(blueprint);
			}
		}

		private static IEnumerable<Blueprint> ParseBlueprints(string path)
		{
			var data = File.ReadAllText(path);

			var settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter>
				{
					new StringToTextureConverter(Path.GetDirectoryName(path) ?? string.Empty),
				},
				MissingMemberHandling = MissingMemberHandling.Error,
				MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			};

			var buildings = JsonConvert.DeserializeObject<SerializedBuildings>(data, settings);

			return buildings?.Data.Values ?? Enumerable.Empty<Blueprint>();
		}

		private record SerializedBuildings([JsonProperty("$schema")] string? Schema,
			IDictionary<string, Blueprint> Data);

		private class StringToTextureConverter : JsonConverter<Texture>
		{
			private readonly string _baseDirectory;

			public StringToTextureConverter(string baseDirectory)
			{
				_baseDirectory = baseDirectory;
			}

			public override void WriteJson(JsonWriter writer, Texture? value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}

			public override Texture? ReadJson(JsonReader reader, Type objectType, Texture? existingValue,
				bool hasExistingValue,
				JsonSerializer serializer)
			{
				if (reader.Value is not string path)
				{
					return null;
				}

				var fullPath = Path.Combine(_baseDirectory, path);

				var image = new Image();

				if (image.Load(fullPath) != Error.Ok)
				{
					throw new FileNotFoundException($"Could not find image {fullPath}");
				}

				var texture = new ImageTexture();
				texture.CreateFromImage(image);
				return texture;
			}
		}
	}
}
