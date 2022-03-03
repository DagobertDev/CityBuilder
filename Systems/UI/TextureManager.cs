using DefaultEcs;
using DefaultEcs.Resource;
using Godot;

namespace CityBuilder.Systems.UI
{
	public class TextureManager : AResourceManager<string, Texture>
	{
		protected override Texture Load(string texturePath)
		{
			var texture = new ImageTexture();

			var image = new Image();

			if (image.Load(texturePath) == Error.Ok)
			{
				texture.CreateFromImage(image, 3);
			}

			return texture;
		}

		protected override void OnResourceLoaded(in Entity entity, string info, Texture texture)
		{
			entity.Set(texture);
		}
	}
}
