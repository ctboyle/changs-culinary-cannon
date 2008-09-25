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

namespace RC.Engine.Test
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        private RCSpatial _sceneRoot = null;


        public TestState(IServiceProvider services)
            : base(services)
        {
        }

        public override void Initialize()
        {
            IRCCameraManager cameraMgr = Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
            IGraphicsDeviceService deviceMgr = Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            IRCRenderManager renderMgr = Services.GetService(typeof(IRCRenderManager)) as IRCRenderManager;
           
            // Create the model

            RCGeometry model = MeshCreator.CreateObject();

            // Create a camera and add it to the camera manageer
            RCCamera camera = new RCPerspectiveCamera(deviceMgr.GraphicsDevice.Viewport);
            camera.LocalTrans = Matrix.Invert(
                Matrix.CreateLookAt(
                    new Vector3(0.0f, 0.0f, 3.0f),
                    Vector3.Zero,
                    Vector3.Up
                )
            );

            cameraMgr.AddCamera("Test", camera);

            
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

        public override void Load()
        {
            _sceneRoot.Load(((IGraphicsDeviceService)this.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice, (ContentManager)Services.GetService(typeof(ContentManager)));
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Draw(GameTime gameTime)
        {           
            IRCRenderManager renderMgr = Services.GetService(typeof(IRCRenderManager)) as IRCRenderManager;
            IRCCameraManager cameraMgr = Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
            
            cameraMgr.SetActiveCamera("Test");

            renderMgr.DrawScene(_sceneRoot);
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput();
            _sceneRoot.UpdateGS(gameTime, true);
        }

        private void UpdateInput()
        {
            
        }
    }
}
