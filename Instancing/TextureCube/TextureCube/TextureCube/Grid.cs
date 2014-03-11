using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace VodacekEngine
{
    public class Grid
    {
        VertexPositionColor[] body;
        int vzdalenost;
        int pocet;

        BasicEffect effect;

        public Color Color
        {
            get;
            set;
        }

        public Grid(int pocet, int vzdalenost)
            : this(pocet, vzdalenost, Color.Black)
        {

        }

        public Grid(int pocet, int vzdalenost, Color c)
        {
            this.vzdalenost = vzdalenost;
            this.pocet = pocet;

            body = new VertexPositionColor[pocet * 4];
            Color = c;
        }

        public void Load(ContentManager content, GraphicsDevice device)
        {
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;

            Vector3 offset = new Vector3(-(pocet * vzdalenost) / 2, 0, -(pocet * vzdalenost) / 2);

            int k = 0;
            for (int i = 0; i < pocet; i++)
            {
                Vector3 tmp = Vector3.Left * vzdalenost * i;
                if (pocet / 2 == i)
                {
                    body[k] = new VertexPositionColor(tmp-offset, Color.Red);
                    body[k + 1] = new VertexPositionColor(tmp + Vector3.Forward * (vzdalenost * (pocet - 1)) - offset, Color.Red);
                    tmp = Vector3.Forward * vzdalenost * i;
                    body[k + 2] = new VertexPositionColor(tmp - offset, Color.Green);
                    body[k + 3] = new VertexPositionColor(tmp + Vector3.Left * (vzdalenost * (pocet - 1)) - offset, Color.Green);
                }
                else
                {
                    body[k] = new VertexPositionColor(tmp - offset, Color);
                    body[k + 1] = new VertexPositionColor(tmp + Vector3.Forward * (vzdalenost * (pocet - 1)) - offset, Color);
                    tmp = Vector3.Forward * vzdalenost * i;
                    body[k + 2] = new VertexPositionColor(tmp - offset, Color);
                    body[k + 3] = new VertexPositionColor(tmp + Vector3.Left * (vzdalenost * (pocet - 1)) - offset, Color);
                }

                k += 4;
            }
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 CameraPosition, GraphicsDevice device)
        {
            effect.View = View;
            effect.Projection = Projection;
            effect.World = Matrix.CreateTranslation(new Vector3(pocet * (vzdalenost / 2), 0, pocet * (vzdalenost / 2)));

            effect.CurrentTechnique.Passes[0].Apply();
            device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, body, 0, body.Length / 2);
        }
    }
}
