using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;
//http://sciencefact.co.uk/2011/12/hardware-geometry-instancing-in-xna-4/ //ulozeno
namespace VodacekEngine
{
    public class InstancedModel3D<T> where T : struct, IInstanceVertexType
    {
        public string ModelName
        {
            get;
            protected set;
        }

        public int MaxCount
        {
            get;
            protected set;
        }

        public int Count
        {
            get;
            protected set;
        }

        protected Model Model;

        protected string effectName;
        protected Effect Effect;

        protected VertexBuffer Verticles;
        protected IndexBuffer Indicies;
        protected DynamicVertexBuffer Primitives;

        protected List<T> PrimitivesList;

        public Texture2D Texture
        {
            get;
            set;
        }

        public InstancedModel3D(string name, int max, string effect)
        {
            ModelName = name;
            MaxCount = max;
            effectName = effect;
            PrimitivesList = new List<T>(MaxCount);
        }

        public virtual void AddPrimitive(T obj)
        {
            PrimitivesList.Add(obj);
        }

        public virtual void RemovePrimitive(T obj)
        {
            PrimitivesList.Remove(obj);
        }

        public virtual void Clear()
        {
            PrimitivesList.Clear();
            Count = 0;
        }

        public virtual void Apply(GraphicsDevice device)
        {
            if (PrimitivesList.Count == 0 || device.GraphicsProfile == GraphicsProfile.Reach) return;

            if (Primitives == null)
            {
                MaxCount = Math.Max(MaxCount, PrimitivesList.Count);
                Primitives = new DynamicVertexBuffer(device, PrimitivesList[0].VertexDeclaration, MaxCount, BufferUsage.None);
                binding[1] = new VertexBufferBinding(Primitives, 0, 1);
            }

            if (MaxCount < PrimitivesList.Count)
            {
                //mame vic nez na co mame buffer
                MaxCount = PrimitivesList.Count;
                Primitives.Dispose();
                Primitives = new DynamicVertexBuffer(device, PrimitivesList[0].VertexDeclaration, MaxCount, BufferUsage.None);
                binding[1] = new VertexBufferBinding(Primitives, 0, 1);
            }

            Primitives.SetData<T>(PrimitivesList.ToArray(), 0, PrimitivesList.Count);
            Count = PrimitivesList.Count;
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            Model = content.Load<Model>(ModelName);
            List<VertexPositionNormalTexture> vert = new List<VertexPositionNormalTexture>();
            List<ushort> ind = new List<ushort>();

            Matrix[] transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    VertexPositionNormalTexture[] partVerts = new VertexPositionNormalTexture[part.VertexBuffer.VertexCount];
                    part.VertexBuffer.GetData(partVerts,part.VertexOffset,part.NumVertices);
                    for (int i = 0; i < partVerts.Length; i++)
                    {
                        partVerts[i].Position = Vector3.Transform(partVerts[i].Position, transforms[mesh.ParentBone.Index]);
                    }
                    ushort[] partIndices = new ushort[part.IndexBuffer.IndexCount];
                    part.IndexBuffer.GetData(partIndices,part.StartIndex,part.PrimitiveCount*3);
                    ind.AddRange(partIndices);

                    vert.AddRange(partVerts);
                }
            }
            Verticles = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vert.Count, BufferUsage.WriteOnly);
            Verticles.SetData<VertexPositionNormalTexture>(vert.ToArray());
            Indicies = new IndexBuffer(device, IndexElementSize.SixteenBits, ind.Count, BufferUsage.WriteOnly);
            Indicies.SetData<ushort>(ind.ToArray());

            Texture = ((BasicEffect)Model.Meshes[0].Effects[0]).Texture;

            //if (device.GraphicsProfile == GraphicsProfile.Reach)
            //{
                Effect = new BasicEffect(device);
            /*}
            else
            {
                //Effect = Parent.Content.Load<Effect>(effectName);
                BinaryReader br = new BinaryReader(File.Open("Content\\" + effectName + ".mgfxo", FileMode.Open));
                Effect = new Effect(device, br.ReadBytes((int)br.BaseStream.Length));
                br.Close();
            }*/

            binding[0] = new VertexBufferBinding(Verticles);
        }

        private readonly VertexBufferBinding[] binding = new VertexBufferBinding[2];

        public virtual void SetEffectParametres(T item)
        {

        }

        public void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition, GraphicsDevice device, int mode)
        {
            bool instantiate = false;
            bool modeldraw = false;

            switch (mode)
            {
                case 0:
                    instantiate = true;
                    break;
                case 1:
                    modeldraw = true;
                    break;
            }

            if (!instantiate)
            {
                BasicEffect ef = Effect as BasicEffect;
                device.SetVertexBuffer(Verticles);
                device.Indices = Indicies;
                ef.TextureEnabled = Texture != null;
                ef.Texture = Texture;
                ef.View = View;
                ef.Projection = Projection;

                foreach (T obj in PrimitivesList)
                {
                    ef.World = obj.GetWorld();
                    SetEffectParametres(obj);

                    if (modeldraw)
                    {
                        Model.Draw(obj.GetWorld(),View,Projection);
                    }
                    else
                    {
                        foreach (EffectPass pass in ef.CurrentTechnique.Passes)
                        {
                            pass.Apply();
                            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                Verticles.VertexCount, 0, Indicies.IndexCount/3);
                        }
                    }
                }
            }
            else
            {
                if (Primitives == null || Count == 0) return;//vyjimku?

                /*Effect.Parameters["View"].SetValue(View);
                Effect.Parameters["Projection"].SetValue(Projection);
                //Effect.Parameters["TextureEnabled"].SetValue(Texture != null);
                Effect.Parameters["Texture"].SetValue(Texture);*/
                BasicEffect effect = Effect as BasicEffect;
                effect.View = View;
                effect.Projection = Projection;
                effect.Texture = Texture;

                Effect.CurrentTechnique.Passes[0].Apply();
                device.SetVertexBuffers(binding);

                device.Indices = Indicies;

                device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, Verticles.VertexCount, 0, Indicies.IndexCount / 3, Count);//trojuhelniky a kolikrat opakovat
            }
        }
    }
}
