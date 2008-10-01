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
        private IRCCameraManager _cameraMgr = null;
        private IRCRenderManager _renderMgr = null;
        private IGraphicsDeviceService _graphics = null;
        private IRCGameStateManager _stateMgr = null;
        private IRCContentRequester _content = null;

        public override void Initialize()
        {
            Graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            // Create the model
            RCGeometry model = MeshCreator.CreateObject(Graphics, Content);

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
            CameraMgr.SetActiveCamera("Test");
            RenderMgr.DrawScene(_sceneRoot);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
        }

        [Inject]
        public IRCContentRequester Content
        {
            get { return _content; }
            set { _content = value; }
        }

        [Inject]
        public IRCGameStateManager StateMgr
        {
            get { return _stateMgr; }
            set { _stateMgr = value; }
        }

        [Inject]
        public IRCCameraManager CameraMgr
        {
            get { return _cameraMgr; }
            set { _cameraMgr = value; }
        }

        [Inject]
        public IRCRenderManager RenderMgr
        {
            get { return _renderMgr; }
            set { _renderMgr = value; }
        }

        [Inject]
        public IGraphicsDeviceService Graphics
        {
            get { return _graphics; }
            set { _graphics = value; }
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