using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VodacekEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VodacekVyvojovaKniha1
{
    public struct InstanceDataVertex: IInstanceVertexType{

        public Matrix World;
        public Color Color;
 
        public InstanceDataVertex(Matrix world, Color colour)
        {
            World = world;
            Color = colour;
        }

        Matrix IInstanceVertexType.GetWorld()
        {
            return World;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return InstanceDataVertex.VertexDeclaration;
            }
        }
 
        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
             new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3),
             new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4),
             new VertexElement(sizeof(float) * 8, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 5),
             new VertexElement(sizeof(float) * 12, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 6),
             //barva
             new VertexElement(sizeof(float) * 16, VertexElementFormat.Color, VertexElementUsage.Color, 0)
 
        );

        public static readonly int SizeInBytes = sizeof(float) * (16 + 4);
    }
}
