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
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero, Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            camera.Near = 0.05f;
            Ctx.CameraMgr.AddCamera("Test", camera);

            /////////////////////////////////////////////////////////////////////
            // Add a controller for the camera
            /////////////////////////////////////////////////////////////////////
            FlyCameraController cameraController = new FlyCameraController(0.05f, MathHelper.PiOver2);
            cameraController.AttachToObject(camera);

            /////////////////////////////////////////////////////////////////////
            // Create and setup the light
            /////////////////////////////////////////////////////////////////////
            RCLightNode lightNode = new RCLightNode();
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            lightNode.SetLight(light);
            lightNode.AddLight(light);

            /////////////////////////////////////////////////////////////////////
            // Create the model
            /////////////////////////////////////////////////////////////////////
            //RCGeometry model = MeshCreator.CreateObject(Ctx.Graphics, Ctx.ContentRqst);
            float heightMapScaling = 5;

            RCContent<RCHeightMap> heightMap = new RCDefaultContent<RCHeightMap>(Ctx.ContentRqst, "Content\\Textures\\heightmap");
            RCContent<Texture2D> texture1 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\grass");
            RCContent<Texture2D> texture2 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\seamless_rock");
            RCContent<Texture2D> texture3 = new RCDefaultContent<Texture2D>(Ctx.ContentRqst, "Content\\Textures\\tileable_snow");
            heightMap.Content.Scaling = heightMapScaling;

            HeightMapEffect effect = new HeightMapEffect(Ctx.ContentRqst, heightMap, texture1, texture2, texture3);
            heightMap.Content.AddEffect(effect);
            

            RCModelContent enemy = new RCModelContent(Ctx.ContentRqst, @"Content\Models\treasure_chest");

            RCDepthBufferState depthState = new RCDepthBufferState();
            depthState.DepthTestingEnabled = true;
            
            enemy.Content.GlobalStates.Add(depthState);
            heightMap.Content.GlobalStates.Add(depthState);

            /////////////////////////////////////////////////////////////////////
            // Setup the light node as the root and setup its children
            /////////////////////////////////////////////////////////////////////
            lightNode.AddChild(camera);
            //lightNode.AddChild(heightMap);
            lightNode.AddChild(enemy);

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
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                Ctx.StateStack.PopState();
            }
        }
    }
}
