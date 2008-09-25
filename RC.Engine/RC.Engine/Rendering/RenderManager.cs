using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RC.Engine.Cameras;
using RC.Engine.GraphicsManagement;
using RC.Engine.Utility;


namespace RC.Engine.Rendering
{
    public delegate void RenderFunc(IRCRenderManager render);

    public enum DirectionalLightIndex
    {
        Light0 = 0,
        Light1,
        Light2,
        Count
    }

    public interface IRCRenderManager : IRCLoadable
    {
        GraphicsDevice Graphics { get; }

        Matrix World
        {
            get;
        }

        Matrix View
        {
            get;
        }
        Matrix Projection
        {
            get;
        }

        void SetRenderState(RCRenderState renderState);
        RCRenderState GetRenderState(RCRenderState.StateType type);
        void Draw(RCGeometry geometry);
        void DrawScene(RCSpatial sceneRoot);
        void ClearScreen();
    }

    /// <summary>
    /// Central functionality for Rendering the Scene.
    /// </summary>
    internal class RCRenderManager : IRCRenderManager
    {
        private IRCCameraManager _cameraMgr = null;
        private Game _game = null;
        private Matrix _world;


        private RCGeometry _geometry = null;

        private RCRenderStateCollection _renderStates =
            new RCRenderStateCollection(true);

        public Matrix World
        {
            get { return _world; }
        }

        public Matrix View
        {
            get { return _cameraMgr.ActiveCamera.View; }
        }
        public Matrix Projection
        {
            get { return _cameraMgr.ActiveCamera.Projection; }
        }


        public RCRenderManager(Game game)
        {
            _game = game;
            _game.Services.AddService(typeof(IRCRenderManager), this);
        }

        public GraphicsDevice Graphics
        {
            get { return _game.GraphicsDevice; }
        }

        public void Load()
        {
            _cameraMgr = _game.Services.GetService(typeof(IRCCameraManager)) as IRCCameraManager;

            IGraphicsDeviceService graphics = _game.Services.GetService(typeof(IGraphicsDeviceService)) as IGraphicsDeviceService;

            graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            graphics.GraphicsDevice.RenderState.StencilEnable = true;
        }

        public void Unload()
        {

        }

        public void SetRenderState(RCRenderStateCollection renderStates)
        {
            foreach (RCRenderState state in renderStates)
            {
                SetRenderState(state);
            }
        }

        public void RestoreRenderState(RCRenderStateCollection renderStates)
        {
            foreach (RCRenderState state in renderStates)
            {
                SetRenderState(RCRenderState.Default[state.GetStateType()]);
            }
        }


        public void SetRenderState(RCRenderState renderState)
        {
            if (renderState != null)
            {
                // Cache the new state
                _renderStates[renderState.GetStateType()] = renderState;

                // Enable the state
                renderState.ConfigureDevice(Graphics);
            }
        }

        public RCRenderState GetRenderState(RCRenderState.StateType type)
        {
            return _renderStates[type];
        }


        /// <summary>
        /// Use to render a scene.
        /// 
        /// Will ensure that the Correct camera and viewport are used!
        /// </summary>
        public void DrawScene(RCSpatial sceneRoot)
        {
            if (_cameraMgr.ActiveCamera == null)
            {
                throw new InvalidOperationException("Active camera must be set before drawing scene");
            }

            UpdateSceneCameraParameters();
            
            // Clear screen using current clear color.
            if (_cameraMgr.ActiveCamera.ClearScreen)
            {
                ClearScreen();
            }

                            
            sceneRoot.Draw(this);
        }

        public void Draw(RCGeometry geometry)
        {
            _geometry = geometry;

            // Enable the geometry's renderstates
            SetRenderState(geometry.RenderStates);

            _world = geometry.WorldTrans;

            // Render the geometry obejct with each of the effects.
            bool isPrimaryEffect = true;
            foreach (RCEffect effect in geometry.Effects)
            {
                ApplyEffect(effect, isPrimaryEffect);
                isPrimaryEffect = false;
            }
        }

        private void ApplyEffect(RCEffect rcEffect, bool isPrimaryEffect)
        {
            Effect shader = rcEffect.Effect;

            // Configure The Effect
            rcEffect.CustomConfigure(this);

            shader.Begin();

            EffectPassCollection passes = shader.CurrentTechnique.Passes;
            for (int iPass = 0; iPass < passes.Count; iPass++ )
            {
                rcEffect.SetRenderState(iPass, this, isPrimaryEffect);

                passes[iPass].Begin();

                DrawElements();

                passes[iPass].End();

                rcEffect.RestoreRenderState(iPass, this, isPrimaryEffect);
            }

            shader.End();
        }

        private void DrawElements()
        {
            RCVertexBuffer VBuffer = _geometry.VBuffer;

            Graphics.VertexDeclaration = VBuffer.VertexDeclaration;
            Graphics.Vertices[0].SetSource(
                VBuffer.VertexBuffer,
                0,
                VBuffer.VertexSize);

            Graphics.Indices = _geometry.IBuffer;
            

            // Finally draw the actual triangles on the screen
            Graphics.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0, 0,
                VBuffer.NumVertices,
                0,
                _geometry.IBuffer.SizeInBytes / (sizeof(short) *3)); // TODO: CHANGE THIS LAST PARAMETER!
        }

        public void ClearScreen()
        {
            Graphics.Clear(
                _cameraMgr.ActiveCamera.ClearOptions,
                _cameraMgr.ActiveCamera.ClearColor,
                1.0f,
                0
                );
        }

        protected bool UpdateSceneCameraParameters()
        {
            // Ensure that the correct viewport is drawn to.
            Graphics.Viewport = _cameraMgr.ActiveCamera.Viewport;
            return true;
        }
    }
}
