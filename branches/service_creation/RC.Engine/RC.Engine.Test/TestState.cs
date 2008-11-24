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
using Ninject.Core;
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
            // Load SpriteBatch Stuff
            _spriteBatch = new RCSpriteBatch(Graphics);
            _spriteBatch.Enabled = true;
            _spriteFont = new RCDefaultContent<SpriteFont>(ContentRqst, "Content\\Fonts\\DefaultFont");

            Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            // Create the model
            RCGeometry model = MeshCreator.CreateObject(Graphics, ContentRqst);

            // Create a camera and add it to the camera manageer
            RCCamera camera = new RCPerspectiveCamera(Graphics.GraphicsDevice.Viewport);
            camera.LocalTrans = Matrix.Invert(
                Matrix.CreateLookAt(
                    new Vector3(0.0f, 0.0f, 3.0f),
                    Vector3.Zero,
                    Vector3.Up
                )
            );

            CameraMgr.AddCamera("Test", camera);

            // Create the directional light
            RCLightNode lightNode = new RCLightNode();
            RCLight light = new RCLight();

            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            light.Transform = Matrix.Invert(
               Matrix.CreateLookAt(
                   new Vector3(0.0f, 0.0f, 3.0f),
                   Vector3.Zero,
                   Vector3.Up
               ));

            lightNode.SetLight(light);
            lightNode.AddLight(light);

            // Structure the scene graph and set the light node as the root
            lightNode.AddChild(camera);
            lightNode.AddChild(model);
            _sceneRoot = lightNode;

            camera.Near = 0.05f;

            FlyCameraController cameraController = new FlyCameraController(5.0f, MathHelper.PiOver2);
            cameraController.AttachToObject(camera);
            
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
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
        }

        private void UpdateInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                StateMgr.PopState();
                StateMgr.PushState("Test");
            }
        }
    }
}