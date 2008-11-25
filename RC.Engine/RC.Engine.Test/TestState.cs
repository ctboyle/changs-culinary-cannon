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

namespace RC.Engine.Test
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        private RCSpatial _sceneRoot = null;
        private TimeSpan _lastFrameReportTime = TimeSpan.Zero;
        private int _frameCount = 0;
        private int _framesPerSecond = 0;
        private RCSpriteBatch _spriteBatch = null;
        private RCContent<SpriteFont> _spriteFont = null;

        public override void Initialize()
        {
            /////////////////////////////////////////////////////////////////////
            // Setup the graphics device
            /////////////////////////////////////////////////////////////////////
            Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            /////////////////////////////////////////////////////////////////////
            // Setup the SpriteBatch content for displaying the FPS text
            /////////////////////////////////////////////////////////////////////
            _spriteBatch = new RCSpriteBatch(Graphics);
            _spriteBatch.Enabled = true;
            _spriteFont = new RCDefaultContent<SpriteFont>(ContentRqst, "Content\\Fonts\\DefaultFont");

            /////////////////////////////////////////////////////////////////////
            // Create and setup the camera
            /////////////////////////////////////////////////////////////////////
            RCCamera camera = new RCPerspectiveCamera(Graphics.GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero, Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            camera.Near = 0.05f;
            CameraMgr.AddCamera("Test", camera);

            /////////////////////////////////////////////////////////////////////
            // Add a controller for the camera
            /////////////////////////////////////////////////////////////////////
            FlyCameraController cameraController = new FlyCameraController(5.0f, MathHelper.PiOver2);
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
            RCGeometry model = MeshCreator.CreateObject(Graphics, ContentRqst);

            /////////////////////////////////////////////////////////////////////
            // Setup the light node as the root and setup its children
            /////////////////////////////////////////////////////////////////////
            lightNode.AddChild(camera);
            lightNode.AddChild(model);
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

            CameraMgr.SetActiveCamera("Test");
            RenderMgr.DrawScene(_sceneRoot);

            // Draw the scene statistics
            string message = string.Format("FPS: {0}\n", _framesPerSecond);

            _spriteBatch.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.SaveState);
            _spriteBatch.SpriteBatch.DrawString(_spriteFont.Content, message, Vector2.Zero, Color.Yellow);
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
                StateStack.PopState();
            }
        }
    }
}