/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated April 5, 2025. Replaces all prior versions.
 *
 * Copyright (c) 2013-2025, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THE SPINE RUNTIMES ARE PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES,
 * BUSINESS INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THE SPINE RUNTIMES, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using Microsoft.Xna.Framework.Graphics;
using Icculus.PhysFS.NET;

namespace Spine
{
    public class PhysFsTextureLoader : TextureLoader
    {
        private readonly GraphicsDevice _device;
        private readonly string[]? _textureLayerSuffixes = null;

        public PhysFsTextureLoader(
            GraphicsDevice device,
            bool loadMultipleTextureLayers = false,
            string[]? textureSuffixes = null)
        {
            _device = device;
            
            if (loadMultipleTextureLayers)
            {
                _textureLayerSuffixes = textureSuffixes;
            }
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

                for (int layer = 1; layer < textureLayersArray.Length; ++layer)
                {
                    string layerPath = GetLayerName(path, _textureLayerSuffixes[0], _textureLayerSuffixes[layer]);
                    textureLayersArray[layer] = LoadTexture(layerPath);
                }
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

        private string GetLayerName(string firstLayerPath, string firstLayerSuffix, string replacementSuffix)
        {
            int suffixLocation = firstLayerPath.LastIndexOf(firstLayerSuffix + ".");
            if (suffixLocation == -1)
            {
                throw new Exception(string.Concat("Error composing texture layer name: first texture layer name '", firstLayerPath,
                                "' does not contain suffix to be replaced: '", firstLayerSuffix, "'"));
            }

            return firstLayerPath.Remove(suffixLocation, firstLayerSuffix.Length).Insert(suffixLocation, replacementSuffix);
        }
    }
}
