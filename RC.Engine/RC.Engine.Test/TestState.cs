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
using RC.Engine.StateManagement;

namespace RC.Engine.Test
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        private RCSpatial _sceneRoot = null;
        private Wedge _wedge = new Wedge();

        public TestState(IServiceProvider services)
            : base(services)
        {
        }
        
        public override void Load()
        {
            // Load the wedge to the draw to the screen
            _wedge.Load(ContentMgr);
         
            // Create a camera and add it to the camera manageer
            RCCamera camera = new RCPerspectiveCamera(Graphics.Viewport);
            camera.LocalTrans = Matrix.Invert(
                Matrix.CreateLookAt(
                    new Vector3(1.0f, 1.0f, 10.0f),
                    Vector3.Zero,
                    Vector3.Up
                )
            );
            CameraMgr.AddCamera("Test", camera);

            // Create the directional light
            RCDirectionalLight lightNode = new RCDirectionalLight(DirectionalLightIndex.Light0);
            lightNode.Diffuse = new Vector3(1.2f);
            lightNode.Specular = new Vector3(0.8f);
            lightNode.LightSource.LocalTrans = Matrix.Invert(
               Matrix.CreateLookAt(
                   new Vector3(1.0f, 1.0f, 10.0f),
                   Vector3.Zero,
                   Vector3.Up
               )
            );

            // Structure the scene graph and set the light node as the root
            lightNode.AddChild(_wedge);
            lightNode.AddChild(camera);
            camera.AddChild(lightNode.LightSource);
            _sceneRoot = lightNode;
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            CameraMgr.SetActiveCamera("Test");
            RenderMgr.DrawScene(_sceneRoot);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
        }

        protected IRCCameraManager CameraMgr
        {
            get { return Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager; }
        }

        protected IRCRenderManager RenderMgr
        {
            get { return Services.GetService(typeof(IRCRenderManager)) as IRCRenderManager; }
        }

        private void UpdateInput()
        {
            KeyboardState newState = Keyboard.GetState();

            Vector3 moveVector = Vector3.Zero;
            float xRot = 0.0f;
            float yRot = 0.0f;

            if (newState.IsKeyDown(Keys.A) && newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Z -= 1;
            }
            else if (newState.IsKeyDown(Keys.A) && !newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Z += 1;
            }

            if (newState.IsKeyDown(Keys.Up) && !newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Y += 1;
            }
            else if (newState.IsKeyDown(Keys.Up) && newState.IsKeyDown(Keys.LeftShift))
            {
                xRot += 0.001f;
            }

            if (newState.IsKeyDown(Keys.Down) && !newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.Y -= 1;
            }
            else if (newState.IsKeyDown(Keys.Down) && newState.IsKeyDown(Keys.LeftShift))
            {
                xRot -= 0.001f;
            }

            if (newState.IsKeyDown(Keys.Left) && !newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.X -= 1;
            }
            else if (newState.IsKeyDown(Keys.Left) && newState.IsKeyDown(Keys.LeftShift))
            {
                yRot += 0.001f;
            }

            if (newState.IsKeyDown(Keys.Right) && !newState.IsKeyDown(Keys.LeftShift))
            {
                moveVector.X += 1;
            }
            else if (newState.IsKeyDown(Keys.Right) && newState.IsKeyDown(Keys.LeftShift))
            {
                yRot += 0.001f;
            }
           
            _wedge.LocalTrans *= 
                Matrix.CreateTranslation(moveVector) * 
                Matrix.CreateRotationX(xRot) * 
                Matrix.CreateRotationY(yRot);
        }
    }
}