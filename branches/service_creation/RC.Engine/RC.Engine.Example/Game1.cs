using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using RC.Engine.Base;
using RC.Engine.ContentManagement;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using RC.Engine.GraphicsManagement;
using RC.Engine.ContentManagement.ContentTypes;
using RC.Engine.SceneEffects;

namespace RC.Engine.Example
{
    public class Game1 : RCXnaGame
    {
        private IRCCameraManager cameraMgr = null;
        private IRCRenderManager renderMgr = null;
        private RCSceneNode sceneRoot = null;

        protected override void Initialize()
        {
            base.Initialize();

            _deviceMgr.PreferredBackBufferWidth = 800;
            _deviceMgr.PreferredBackBufferHeight = 600;
            
            #region Get Required Services
            cameraMgr = (IRCCameraManager)Services.GetService(typeof(IRCCameraManager));
            renderMgr = (IRCRenderManager)Services.GetService(typeof(IRCRenderManager));
            #endregion

            // Create the Root Node
            sceneRoot = new RCSceneNode();

            // Create a Camera
            RCCamera camera = new RCPerspectiveCamera(GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(0.0f, 40.0f, 10.0f), -Vector3.UnitZ, Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            cameraMgr.AddCamera("MainCamera", camera);

            // Create the light
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.2f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 0f), Vector3.UnitZ, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            sceneRoot.AddLight(light);

            // Create the model
            RCModelContent model = new RCModelContent(@"Content\enemy");
            sceneRoot.AddChild(model);
        }

        protected override void Update(GameTime gameTime)
        {
            sceneRoot.UpdateGS(gameTime, true);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            cameraMgr.SetActiveCamera("MainCamera");
            sceneRoot.Draw(renderMgr);
            base.Draw(gameTime);
        }
    }
}
