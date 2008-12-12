using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework.Input;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement;
using RC.Engine.StateManagement;
using RC.Engine.Base;
using RC.Physics;
using RC.Engine.GraphicsManagement.BoundingVolumes;
using RC.Content.Heightmap;
using RC.Engine.ContentManagement.ContentTypes;
using JigLibX.Math;
using JigLibX.Vehicles;
using JigLibX.Physics;

namespace RC.Engine.Test
{
    class TestState : RCGameState
    {
        private RCSpatial _sceneRoot = null;
        private TimeSpan _lastFrameReportTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private int _framesPerSecond = 0;
        private RCSpriteBatch _spriteBatch = null;
        private RCContent<SpriteFont> _spriteFont = null;

        public TestState(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
            RCSceneNode sceneRoot = new RCSceneNode();

            /////////////////////////////////////////////////////////////////////
            // Setup the graphics device
            /////////////////////////////////////////////////////////////////////
            Ctx.Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            /////////////////////////////////////////////////////////////////////
            // Setup the SpriteBatch content for displaying the FPS text
            /////////////////////////////////////////////////////////////////////
            _spriteBatch = new RCSpriteBatch();
            _spriteBatch.Enable(Ctx.Graphics);
            _spriteFont = new RCDefaultContent<SpriteFont>(Ctx.ContentRqst, "Content\\Fonts\\DefaultFont");

            /////////////////////////////////////////////////////////////////////
            // Create and setup the camera
            /////////////////////////////////////////////////////////////////////
            RCCamera camera = new RCPerspectiveCamera(Ctx.Graphics.GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 40.0f, 10.0f), -Vector3.UnitZ, Vector3.Up);
            Vector3 truckStartPosition = new Vector3(-10.4f, 12.8f, -25.15f);
            Matrix truckStartCam = Matrix.CreateLookAt(truckStartPosition, 
                truckStartPosition + new Vector3(-.55f,-0.4f,.5f), Vector3.Up);

            //Matrix upLookingDown = Matrix.CreateLookAt(new Vector3(2.5f, 40f, -0.5f), new Vector3(-0.8f,15f,-5.5f), Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            //camera.LocalTrans = Matrix.Invert(upLookingDown);
            //camera.LocalTrans = Matrix.Invert(truckStartCam);
            camera.Near = 0.05f;
            Ctx.CameraMgr.AddCamera("Test", camera);

            /////////////////////////////////////////////////////////////////////
            // Add a controller for the camera
            /////////////////////////////////////////////////////////////////////
            FlyCameraController cameraController = new FlyCameraController(5f, MathHelper.PiOver2);
            cameraController.AttachToObject(camera);

            /////////////////////////////////////////////////////////////////////
            // Create and setup the light
            /////////////////////////////////////////////////////////////////////
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 0f), Vector3.UnitZ, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);

            /////////////////////////////////////////////////////////////////////
            // Create the heightmap
            /////////////////////////////////////////////////////////////////////
            JibLibXObject heightmap = CreateHeightmap(50);

            /////////////////////////////////////////////////////////////////////
            // Create the models
            /////////////////////////////////////////////////////////////////////
            JigLibXVehicle car = CreateVehicle();

            /////////////////////////////////////////////////////////////////////
            // Setup the light node as the root and setup its children
            /////////////////////////////////////////////////////////////////////
            sceneRoot.AddLight(light);
            sceneRoot.AddChild(camera);
            sceneRoot.AddChild(heightmap);
            sceneRoot.AddChild(car);

            RCDepthBufferState depthState1 = new RCDepthBufferState();
            depthState1.DepthTestingEnabled = true;
            sceneRoot.GlobalStates.Add(depthState1);

            _sceneRoot = sceneRoot;
            _sceneRoot.UpdateRS();

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            _lastFrameReportTime += gameTime.ElapsedGameTime;
            _frameCount++;

            if (_lastFrameReportTime.TotalSeconds >= 1.0f)
            {
                _framesPerSecond = _frameCount;
                _frameCount = 0;
                _lastFrameReportTime = TimeSpan.Zero;
            }

            Ctx.CameraMgr.SetActiveCamera("Test");
            Ctx.RenderMgr.Draw(_sceneRoot);

            // Draw the scene statistics
            string message = string.Format("FPS: {0}\n", _framesPerSecond);

            _spriteBatch.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
            _spriteBatch.SpriteBatch.DrawString(_spriteFont, message, Vector2.Zero, Color.Yellow);
            _spriteBatch.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
            base.Update(gameTime);
        }

        private JibLibXObject CreateHeightmap(float heightMapScaling)
        {
            RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\final_heightmap");
            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tilable_long_grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
            heightMap.Content.Scaling = heightMapScaling;

            HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap,
                texture1, texture2, texture3, .3f, .55f, .65f, .75f);
            heightMap.Content.AddEffect(effect);

            JibLibXObject physicsHeightMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap);

            return physicsHeightMap;
        }

        private JigLibXVehicle CreateVehicle()
        {
            RCModelContent wheelDrawable1 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable2 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable3 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent wheelDrawable4 = new RCModelContent(Ctx.ContentRqst, @"Content\Models\wheel");
            RCModelContent carDrawable = new RCModelContent(Ctx.ContentRqst, @"Content\Models\car");
            carDrawable.Content.WorldTrans = Matrix.CreateTranslation(new Vector3(0, 50f, 0));

            JigLibXWheel wheel1 = new JigLibXWheel(wheelDrawable1);
            JigLibXWheel wheel2 = new JigLibXWheel(wheelDrawable2);
            JigLibXWheel wheel3 = new JigLibXWheel(wheelDrawable3);
            JigLibXWheel wheel4 = new JigLibXWheel(wheelDrawable4);

            Car car = new Car(true, true, 30.0f, 5.0f, 4.7f, 5.0f, 0.20f, 0.4f, 0.05f,
                0.45f, 0.3f, 1, 520.0f, PhysicsSystem.CurrentPhysicsSystem.Gravity.Length());

            JigLibXVehicle carPhysics = new JigLibXVehicle(car, carDrawable, wheel1, wheel2, wheel3, wheel4);

            return carPhysics;
        }

        private void UpdateInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
            {
                Ctx.StateStack.PopState();
            }
        }
    }
}
