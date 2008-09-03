using System;
using System.Collections.Generic;
using System.Text;
using RC.Engine.GraphicsManagement;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Cameras;
using RC.Engine.Rendering;
using Microsoft.Xna.Framework;

namespace ChangsCulinaryCannon
{
    class TestState : RC.Engine.StateManagement.RCGameState
    {
        RCNode _rootNode = null;

        public override void Initialize(IServiceProvider services)
        {
            IGraphicsDeviceService graphics = services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;
            IRCCameraManager cameraMgr = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;

            RCCamera enemyCamera = new RCPerspectiveCamera(graphics.GraphicsDevice.Viewport);
           
            enemyCamera.LocalTrans = Matrix.Invert(
                Matrix.CreateLookAt(
                    new Vector3(10.0f, 10.0f, 10.0f),
                    Vector3.Zero,
                    Vector3.Up
                )
            );
            
            cameraMgr.Add("EnemyCamera", enemyCamera);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, IServiceProvider services)
        {
            RCModelNode model = new RCModelNode(@"Content//enemy", EnemyEffect);

            model.LoadContent(content, services);

            _rootNode = model;
        }

        public override void UnloadContent()
        {
            
        }

        public override void StateChanged(RC.Engine.StateManagement.RCGameState newState, RC.Engine.StateManagement.RCGameState oldState)
        {
            
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime, IServiceProvider services)
        {
            IRCCameraManager cameraMgr = services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;
            IRCRenderManager render = services.GetService(typeof(IRCRenderManager)) as IRCRenderManager;
            
            cameraMgr.SetActiveCamera("EnemyCamera");
            render.RenderScene(gameTime, _rootNode);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
           
        }

        private void EnemyEffect(RCCamera camera, RCNode callingNode, ModelMesh mesh, Effect effect)
        {
            BasicEffect basicEffect = effect as BasicEffect;

            if (basicEffect == null) return;

            basicEffect.EnableDefaultLighting();
            basicEffect.World = camera.WorldTrans;
            basicEffect.View = camera.View;
            basicEffect.Projection = camera.Projection;
        }
    }
}
