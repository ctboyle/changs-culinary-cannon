using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using RC.Engine.GraphicsManagement;
using RC.Engine.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Rendering;

namespace RC.Engine.Test
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        private RCSpatial _sceneRoot;

        public override void Load(IServiceProvider services)
        {
            Wedge wedge = new Wedge();
            wedge.Load(services);

            IRCCameraManager cameraMgr = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
            IGraphicsDeviceService deviceMgr = services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            IRCRenderManager renderMgr = services.GetService(typeof(IRCRenderManager)) as IRCRenderManager;

            RCCamera camera = new RCPerspectiveCamera(deviceMgr.GraphicsDevice.Viewport);

            cameraMgr.AddCamera("Test", camera);

            camera.LocalTrans = Matrix.Invert(
                Matrix.CreateLookAt(
                    new Vector3(10.0f, 10.0f, 1000.0f),
                    Vector3.Zero,
                    Vector3.Up
                )
            );

            RCDirectionalLight lightNode = new RCDirectionalLight(
                DirectionalLightIndex.Light0
            );

            lightNode.AddChild(wedge);
            lightNode.AddChild(camera);

            lightNode.LightSource.LocalTrans = Matrix.Invert(
               Matrix.CreateLookAt(
                   new Vector3(1.0f, 1.0f, 10.0f),
                   Vector3.Zero,
                   Vector3.Up
               )
            );

            camera.AddChild(lightNode.LightSource);

            lightNode.Diffuse = new Vector3(1.2f);
            lightNode.Specular = new Vector3(0.8f);

            _sceneRoot = lightNode;

            base.Load(services);
        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, IServiceProvider services)
        {
            IRCRenderManager renderMgr = services.GetService(typeof(IRCRenderManager)) as IRCRenderManager;
            IRCCameraManager cameraMgr = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
            cameraMgr.SetActiveCamera("Test");
            renderMgr.DrawScene(_sceneRoot);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, IServiceProvider services)
        {
            _sceneRoot.UpdateGS(gameTime, false);
        }
    }
}