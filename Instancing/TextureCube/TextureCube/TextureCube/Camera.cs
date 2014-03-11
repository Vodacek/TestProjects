using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace VodacekEngine
{
    public class FreeKamera
    {
        float fYaw, fPitch;

        public float Yaw
        {
            get
            {
                return fYaw;
            }
            set
            {
                fYaw = value;
            }
        }

        public float Pitch
        {
            get
            {
                return fPitch;
            }
            set
            {
                if (value > MathHelper.PiOver2 - 0.01) return;
                if (value < -(MathHelper.PiOver2 - 0.01)) return;
                fPitch = value;
            }
        }

        public Matrix ViewTransform
        {
            get
            {
                return Matrix.CreateFromYawPitchRoll(fYaw, fPitch, 0);
            }
        }

        public Vector3 Position;
        private Vector3 Target;

        public Matrix View;
        public Matrix Projection;

        public FreeKamera(Vector3 pozice, Vector3 target, GraphicsDevice device)
        {
            fYaw = 0;
            fPitch = 0;
            Position = pozice;
            Target = target;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                device.Viewport.AspectRatio, 1f, 1000);
        }

        private MouseState oldMouse;

        public void UpdateView(GameTime time)
        {
            MouseState mouse = Mouse.GetState();
            Yaw -= (mouse.X-oldMouse.X) * 0.01f;
            Pitch -= (mouse.Y - oldMouse.Y) * 0.01f;

            Matrix rotace = Matrix.CreateFromYawPitchRoll(fYaw, fPitch, 0);

            Vector3 posun = Vector3.Zero;

            KeyboardState keyb = Keyboard.GetState();

            if (keyb.IsKeyDown(Keys.Up)) posun += Vector3.Forward;
            if (keyb.IsKeyDown(Keys.Down)) posun += Vector3.Backward;
            if (keyb.IsKeyDown(Keys.Left)) posun += Vector3.Left;
            if (keyb.IsKeyDown(Keys.Right)) posun += Vector3.Right;

            posun *= 0.1f * (float)time.ElapsedGameTime.TotalMilliseconds;

            Position += Vector3.Transform(posun, rotace);

            Target = Position + Vector3.Transform(Vector3.Forward, rotace);
            oldMouse = mouse;

            Vector3 dir = Position - Target;
            dir.Normalize();// --------------------------------------
            this.View = Matrix.CreateLookAt(Position, Target, dir != Vector3.Up ? dir != Vector3.Down ? Vector3.Up : Vector3.Left : Vector3.Left);
        }
    }
}
