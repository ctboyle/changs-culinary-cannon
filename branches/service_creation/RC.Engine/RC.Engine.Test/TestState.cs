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
        private JibLibXPhysicsObject physicsEnemy = null;

        public TestState(RCGameContext gameCtx)
            : base(gameCtx)
        {
        }

        public override void Initialize()
        {
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
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 0.0f), -Vector3.UnitZ, Vector3.Up);
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
            RCLightNode lightNode = new RCLightNode();
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 0f), Vector3.UnitZ, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            lightNode.SetLight(light);
            lightNode.AddLight(light);

            /////////////////////////////////////////////////////////////////////
            // Create the model
            /////////////////////////////////////////////////////////////////////
            float heightMapScaling = 10;

            RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\final_heightmap");
            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tilable_long_grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
            heightMap.Content.Scaling = heightMapScaling;

            HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap,
                texture1, texture2, texture3, .3f, .55f, .65f, .75f);
            heightMap.Content.AddEffect(effect);
            JibLibXPhysicsObject physicsHeightMap = JibLibXPhysicsHelper.CreateHeightmap(heightMap);
            
            /////////////////////////////////////////////////////////////////////
            // Create the models
            /////////////////////////////////////////////////////////////////////

            for (int i = 0; i < 25; ++i)
            {
                RCModelContent enemy = new RCModelContent(Ctx.ContentRqst, @"Content\Models\treasure_chest");
                enemy.Content.WorldTrans = Matrix.CreateTranslation(new Vector3(0,20f+(float)2*i,3f));
                physicsEnemy = JibLibXPhysicsHelper.CreateObject(enemy);
                physicsEnemy.SetMass(10.0f);
                JigLibX.Geometry.Box box = new JigLibX.Geometry.Box(-0.5f * new Vector3(0.5f), Matrix.Identity, new Vector3(0.5f));
                physicsEnemy.Body.CollisionSkin.AddPrimitive(
                    box,
                    (int)JigLibX.Collision.MaterialTable.MaterialID.UserDefined,
                    new JigLibX.Collision.MaterialProperties(0.2f, 0.9f, 0.9f)
                    );

                RCDepthBufferState depthState = new RCDepthBufferState();
                depthState.DepthTestingEnabled = true;
                enemy.Content.GlobalStates.Add(depthState);

                lightNode.AddChild(physicsEnemy);
            }

            //RCModelContent car = new RCModelContent(Ctx.ContentRqst, @"Content\Models\car");
            
            /////////////////////////////////////////////////////////////////////
            // Setup the light node as the root and setup its children
            /////////////////////////////////////////////////////////////////////
            lightNode.AddChild(camera);
            lightNode.AddChild(physicsHeightMap);

            _sceneRoot = lightNode;
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

        private void UpdateInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Ctx.StateStack.PopState();
            }
            
        }
    }
}
