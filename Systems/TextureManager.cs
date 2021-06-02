using System;
using DefaultEcs;
using DefaultEcs.Resource;
using Godot;

namespace CityBuilder.Systems
{
	public class TextureManager : AResourceManager<string, Texture>
	{
		private static readonly Lazy<TextureManager> LazyInstance = new(() => new TextureManager());
		public static TextureManager Instance => LazyInstance.Value;
		private TextureManager() {}

		protected override Texture Load(string texturePath)
		{
			var texture = new ImageTexture();

			var image = new Image();

			if (image.Load(texturePath) == Error.Ok)
			{
				texture.CreateFromImage(image);
			}

			return texture;
		}

		protected override void OnResourceLoaded(in Entity entity, string info, Texture texture)
		{
			entity.Set(texture);
		}
	}
}
