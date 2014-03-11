using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VodacekEngine
{
    public interface IInstanceVertexType : IVertexType
    {
        Matrix GetWorld();
    }
}
