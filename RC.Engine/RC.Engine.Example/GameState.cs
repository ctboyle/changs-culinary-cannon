using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.StateManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework;
using RC.Engine.SceneEffects;
using RC.Engine.ContentManagement.ContentTypes;
using Microsoft.Xna.Framework.Graphics;

namespace RC.Engine.Example
{
    class GameState : RCGameState
    {
        private IGraphicsDeviceService graphics = null;
        private IRCCameraManager cameraMgr = null;
        private IRCRenderManager renderMgr = null;
        private RCSceneNode sceneRoot = null;

        public GameState(IServiceProvider services)
            : base(services)
        {
            #region Get Required Services
            graphics = (IGraphicsDeviceService)Services.GetService(typeof(IGraphicsDeviceService));
            cameraMgr = (IRCCameraManager)Services.GetService(typeof(IRCCameraManager));
            renderMgr = (IRCRenderManager)Services.GetService(typeof(IRCRenderManager));
            #endregion
        }

        public override void Initialize()
        {
            // Create the Root Node
            sceneRoot = new RCSceneNode();

            // Create a Camera
            RCCamera camera = new RCPerspectiveCamera(graphics.GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 40.0f, 10.0f), Vector3.Zero, Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            cameraMgr.AddCamera("MainCamera", camera);

            // Create the light
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 0f), Vector3.Zero, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            sceneRoot.AddLight(light);

            // Create the model
            RCModelContent model = new RCModelContent(@"Content\enemy");
            sceneRoot.AddChild(model);

            base.Initialize();
        }
        
        public override void Update(GameTime gameTime)
        {
            sceneRoot.UpdateGS(gameTime, true);
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            cameraMgr.SetActiveCamera("MainCamera");
            sceneRoot.Draw(renderMgr);
            base.Draw(gameTime);
        }
    }
}
