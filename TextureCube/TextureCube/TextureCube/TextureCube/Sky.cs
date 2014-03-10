using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace TextureCubeTest
{
    public class SkySphere
    {
        TextureCube Texture;

        string textureName;

        Model Model;
        Effect effect;
        Matrix World;

        public SkySphere(string textura)
        {
            textureName = textura;
        }
        public void Load(ContentManager content, GraphicsDevice device)
        {
            Texture = content.Load<TextureCube>(textureName);

            World = Matrix.CreateScale(new Vector3(100));
            Model = content.Load<Model>("skysphere_mesh");

            //effect = Parent.Content.Load<Effect>("SkySphere2");

            BinaryReader br = new BinaryReader(File.Open("Content\\SkySphere2.mgfxo", FileMode.Open));
            effect = new Effect(device, br.ReadBytes((int)br.BaseStream.Length));
            br.Close();

            effect.Parameters["CubeMap"].SetValue(Texture);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
            //SetModelEffect(effect, false);
        }
        public void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition, GraphicsDevice device)
        {
            //Parent.Engine.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["CameraPosition"].SetValue(CameraPosition);

            Matrix wo = Matrix.CreateScale(200) * Matrix.CreateTranslation(CameraPosition);

            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            //Parent.Engine.GraphicsDevice.RasterizerState = StateObjects.WireState;

            foreach (ModelMesh mesh in Model.Meshes)
            {
                effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * wo);
                mesh.Draw();
            }

            //Parent.Engine.GraphicsDevice.RasterizerState = StateObjects.SolidState;
            //Parent.Engine.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            device.SamplerStates[0] = SamplerState.LinearWrap;
        }
    }
}
