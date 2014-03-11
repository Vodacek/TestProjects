using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using VodacekEngine;
using VodacekVyvojovaKniha1;

namespace TextureCubeTest
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private SkySphere sky;
        private InstancedModel3D<InstanceDataVertex> instances;
        private FreeKamera camera;
        private Grid grid;
        private SpriteFont font;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            sky=new SkySphere("clouds");
            instances=new InstancedModel3D<InstanceDataVertex>("zed",10,"instancedeffect");
            grid=new Grid(100,1,Color.White);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            sky.Load(Content,GraphicsDevice);
            instances.Load(Content,GraphicsDevice);
            instances.AddPrimitive(new InstanceDataVertex(Matrix.CreateTranslation(new Vector3(5,10,0)),Color.Red));
            instances.AddPrimitive(new InstanceDataVertex(Matrix.CreateScale(0.3f)* Matrix.CreateTranslation(new Vector3(0)), Color.Red));
            instances.Apply(GraphicsDevice);

            grid.Load(Content,GraphicsDevice);

            camera=new FreeKamera(new Vector3(),Vector3.Forward,GraphicsDevice);

            font = Content.Load<SpriteFont>("Maly");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        private KeyboardState oldkey;

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            camera.UpdateView(gameTime);

            KeyboardState keyb = Keyboard.GetState();
            if (oldkey.IsKeyUp(Keys.Space) && keyb.IsKeyDown(Keys.Space))
            {
                mode++;
                if (mode > 2) mode = 0;
            }
            oldkey = keyb;
            base.Update(gameTime);
        }

        private int mode = 0;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            /*Matrix View = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitX, Vector3.Up);
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2,
                GraphicsDevice.DisplayMode.AspectRatio, 1, 1000);*/

            // TODO: Add your drawing code here
            //sky.Draw(camera.View,camera.Projection,camera.Position,GraphicsDevice);

            instances.Draw(camera.View, camera.Projection, camera.Position, GraphicsDevice, mode);

            grid.Draw(camera.View,camera.Projection,camera.Position,GraphicsDevice);

            spriteBatch.Begin();
            spriteBatch.DrawString(font,"Position: "+camera.Position,Vector2.Zero,Color.Red);

            switch (mode)
            {
                case 0:
                    spriteBatch.DrawString(font, "Mode: instancing", new Vector2(600,450), Color.Red);
                    break;
                case 1:
                    spriteBatch.DrawString(font, "Mode: Model.Draw", new Vector2(600, 450), Color.Red);
                    break;
                case 2:
                    spriteBatch.DrawString(font, "Mode: DrawIndexedPrimitives", new Vector2(500, 450), Color.Red);
                    break;
            }
            spriteBatch.End();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(gameTime);
        }
    }
}
