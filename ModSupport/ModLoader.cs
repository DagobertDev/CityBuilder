using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CityBuilder.Components;
using CityBuilder.Core.Components;
using CityBuilder.Core.ModSupport;
using CityBuilder.Systems.UI;
using DefaultEcs;
using DefaultEcs.Resource;
using DefaultEcs.Serialization;
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
		private const string BlueprintFileExtension = "bp";
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
			var serializer = new TextSerializer(new TextSerializationContext());

			var str = new MemoryStream();
			var writer = new StreamWriter(str);

			WriteTypes(writer, typeof(Position).Assembly.GetTypes()
				.Where(t => t.Namespace != null && t.Namespace.Contains("Components") && !t.IsAbstract)
				.Concat(new[] { typeof(BlueprintInfo) }));
			writer.Flush();
			str.Position = 0;

			Entity entity;
			Entity construction;

			using (var stream = new ConcatenatedStream(str, File.OpenRead(path)))
			{
				var entities = serializer.Deserialize(stream, _world);
				entity = entities.FirstOrDefault(e => e.Has<BlueprintInfo>());
				construction = entities.FirstOrDefault(e => e.Has<Construction>());
			}

			if (entity == default)
			{
				return null;
			}

			var info = entity.Get<BlueprintInfo>();
			var texturePath = path.Replace(Path.GetFileName(path), info.Texture);
			entity.Set(ManagedResource<Texture>.Create(texturePath));

			if (construction != default)
			{
				entity.Set(new ConstructionReference(construction));
			}

			return new Blueprint(info.Name, entity, newEntity => new ComponentCloner().Clone(entity, newEntity));
		}

		private static void WriteTypes(TextWriter writer, IEnumerable<Type> types)
		{
			foreach (var type in types)
			{
				writer.WriteLine($"ComponentType {type.Name} {type.FullName}, {type.Assembly.GetName().Name}");
			}
		}

		private class ConcatenatedStream : Stream
		{
			private readonly Queue<Stream> _streams;

			public ConcatenatedStream(params Stream[] streams)
			{
				_streams = new Queue<Stream>(streams);
			}

			public override bool CanRead => true;

			public override int Read(byte[] buffer, int offset, int count)
			{
				var totalBytesRead = 0;

				while (count > 0 && _streams.Count > 0)
				{
					var bytesRead = _streams.Peek().Read(buffer, offset, count);
					if (bytesRead == 0)
					{
						_streams.Dequeue().Dispose();
						continue;
					}

					totalBytesRead += bytesRead;
					offset += bytesRead;
					count -= bytesRead;
				}

				return totalBytesRead;
			}

			public override bool CanSeek => false;
			public override bool CanWrite => false;

			public override void Flush()
			{
				throw new NotImplementedException();
			}

			public override long Length => throw new NotImplementedException();

			public override long Position
			{
				get => throw new NotImplementedException();
				set => throw new NotImplementedException();
			}

			public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
			public override void SetLength(long value) => throw new NotImplementedException();
			public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
		}

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
					return null;
				}

				var texture = new ImageTexture();
				texture.CreateFromImage(image);
				return texture;
			}
		}
	}
}
