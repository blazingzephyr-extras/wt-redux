
using Microsoft.Xna.Framework.Graphics;
using Icculus.PhysFS.NET;
using System.IO;

namespace Spine
{
    public class FnaTextureLoader : TextureLoader
    {
        private readonly GraphicsDevice _device;
        private readonly string[] _textureLayerSuffixes = null;

        public FnaTextureLoader(
            GraphicsDevice device,
            bool loadMultipleTextureLayers = false,
            string[] textureSuffixes = null)
        {
            _device = device;
            if (loadMultipleTextureLayers)
                _textureLayerSuffixes = textureSuffixes;
        }

        public void Load(AtlasPage page, string path)
        {
            Texture2D texture = LoadTexture(path);
            page.width = texture.Width;
            page.height = texture.Height;

            if (_textureLayerSuffixes == null)
            {
                page.rendererObject = texture;
            }
            else
            {
                Texture2D[] textureLayersArray = new Texture2D[_textureLayerSuffixes.Length];
                textureLayersArray[0] = texture;

                // TBA. Requires some minor restructuring.
                //
                // for (int layer = 1; layer < textureLayersArray.Length; ++layer)
                // {
                    // string layerPath = GetLayerName(path, _textureLayerSuffixes[0], _textureLayerSuffixes[layer]);
                    // textureLayersArray[layer] = Util.LoadTexture(device, layerPath);
                // }
                //
                // page.rendererObject = textureLayersArray;
            }
        }

        public void Unload(object texture)
        {
            if (texture is Texture2D fnaTexture)
            {
                fnaTexture.Dispose();
            }
        }

        private Texture2D LoadTexture(string path)
        {
            FileSystemObject file = PhysFS.OpenFile(path, FileSystemObjectAccess.Read);
            byte[] buffer = file.ReadBytes();

            MemoryStream stream = new MemoryStream(buffer);
            Texture2D texture = Texture2D.FromStream(_device, stream);

            stream.Close();
            file.Dispose();

            return texture;
        }
    }
}
