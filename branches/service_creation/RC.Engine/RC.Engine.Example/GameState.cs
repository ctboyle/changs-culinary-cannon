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
            ///////////////////////////////////////////////////////////////
            // Create a Camera
            ///////////////////////////////////////////////////////////////
            RCCamera camera = new RCPerspectiveCamera(graphics.GraphicsDevice.Viewport);
            Matrix cameraLookAt = Matrix.CreateLookAt(new Vector3(15.0f, 15.0f, 15.0f), Vector3.Zero, Vector3.Up);
            camera.LocalTrans = Matrix.Invert(cameraLookAt);
            cameraMgr.AddCamera("MainCamera", camera);

            ///////////////////////////////////////////////////////////////
            // Create the light
            ///////////////////////////////////////////////////////////////
            RCLight light = new RCLight();
            light.Diffuse = new Vector3(1.0f);
            light.Specular = new Vector3(0.8f);
            Matrix lightLookAt = Matrix.CreateLookAt(new Vector3(0f, 25.0f, 25.0f), Vector3.Zero, Vector3.Up);
            light.Transform = Matrix.Invert(lightLookAt);
            

            ///////////////////////////////////////////////////////////////
            // Create the model
            ///////////////////////////////////////////////////////////////
            RCModelContent model = new RCModelContent(@"Content\enemy");

            ///////////////////////////////////////////////////////////////
            // Construt the scene graph.
            ///////////////////////////////////////////////////////////////
            // Create the Root Node
            sceneRoot = new RCSceneNode();

            // ALWAYS INCLUD CAMERA IN THE SCENE GRAPH SO IT CAN BE UPDATTED.
            sceneRoot.AddChild(camera);
            sceneRoot.AddChild(model);

            // Attach a light to the root; this is not a child node.
            sceneRoot.AddLight(light);



            /////////////////////////////////////////////////////////////////
            // Create and add a render state statte to turn on depth testig for
            // our model.
            //////////////////////////////////////////////////////////////////
            RCDepthBufferState depthState = new RCDepthBufferState();
            depthState.DepthTestingEnabled = true;

            model.Content.GlobalStates.Add(depthState);

            // Update the renderstates so that lighting will be applied
            sceneRoot.UpdateRS();

            base.Initialize();
        }
        
        public override void Update(GameTime gameTime)
        {
            // Update the scene graph.
            sceneRoot.UpdateGS(gameTime, true);
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            // Set the active camera
            cameraMgr.SetActiveCamera("MainCamera");

            // Use the render manager to draw the scene.
            renderMgr.Draw(sceneRoot);
            base.Draw(gameTime);
        }
    }
}
